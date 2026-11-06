# 2. Arquitetura de nuvem e DevOps

## 2.1 Arquitetura alinhada à integração

A dificuldade de integração de sistemas, na Alunos Tech, era a matrícula viver numa planilha e o chamado viver noutra caixa de entrada. A arquitetura que responde a isso é uma API REST única, em .NET 8, consumida pelo portal do gestor e pelo cliente móvel do colaborador. Os dois clientes não têm banco próprio e não repetem a regra de duplicidade. Quem integra os fluxos é o contrato HTTP: login, matrícula e chamado. O domínio, a aplicação, a infraestrutura e a interface permanecem em projetos separados, com a regra de negócio independente do PostgreSQL e do HTTP (MARTIN, 2019).

Essa escolha é deliberadamente modular e não é uma malha de microsserviços. Não há repositório de serviços autônomos, nem barramento, nem deploy separado de “serviço de matrícula” e “serviço de chamado”. Há uma aplicação que o negócio reconhece como a plataforma, publicada como uma imagem. A topologia aplicada nesta máquina é um cluster kind de um nó, com a imagem atrás de um Service e o PostgreSQL em ClusterIP, sem porta no host. O Secret guarda a cadeia de conexão e a chave do token. O arquivo está em `deploy/kubernetes` e foi aplicado.

O gerenciamento de informações fica no PostgreSQL 16, sistema de registro de aluno, curso, matrícula e chamado. A escalabilidade neste trabalho não nasce de duplicar planilha: nasce de réplicas da API sem estado e de sondas que só recebem tráfego quando o banco responde. Uma leitura anterior do HPA deste cluster de um nó chegou a seis réplicas com a CPU acima de 70% do pedido; a Figura 11 mostra a leitura posterior, com 2 réplicas e CPU em 2% de 70% (Apêndice A – Cluster local). Isso não é nuvem de provedor.

A Figura 1 reúne essa topologia, sem colar manifesto. O portal do gestor e o aplicativo MAUI falam com a API. A API e o PostgreSQL estão no cluster kind. A imagem aplicada saiu do registro localhost:5001.

[[figura:figuras/arquitetura.png|14.5]]
[[legenda:Figura 1 — Portal, aplicativo MAUI, API, PostgreSQL e registro local]]
[[fonte:Fonte: Elaborada pela autora.]]

## 2.2 Ambiente que executa hoje

O Dockerfile da raiz pretende uma imagem de dois estágios: parte do SDK 8.0.427, publica `src/Api/AlunosTech.Api/AlunosTech.Api.csproj`, inclusive as páginas de `wwwroot`, e fecha em `aspnet:8.0.33`, na porta 8080, com o usuário 1654 (DOCKER, [s.d.]b). Esse build não construiu. A imagem aplicada no cluster saiu de `Dockerfile.cluster`, a partir de `aspnet:10.0`, com o publish net8.0 e roll-forward, e está em `localhost:5001/alunos-tech-api:4604e61` (Apêndice C – Trechos de código).

O Docker Compose sobe somente o PostgreSQL 16 Alpine. Há volume nomeado, rede do projeto, porta presa a 127.0.0.1 e verificação `pg_isready`. A API não é serviço do Compose. Subir o banco não publica a plataforma (DOCKER, [s.d.]a). A API local conecta com o papel `alunos_tech_app`. O superusuário fica para a administração do schema.

O fluxo no GitHub Actions continua só na integração: restaura, compila, testa e constrói com `push` falso. Esse job não publicou a imagem nem atualizou o cluster. A entrega local é a que enviou a tag já citada: o `docker push` gravou o digest `sha256:e7771da68970` e o `kubectl rollout` terminou com as duas réplicas disponíveis. O EOF do daemon, na consulta a `mcr.microsoft.com`, é o motivo de o Dockerfile da raiz não ter sido a imagem aplicada.

## 2.3 Segurança da API e das credenciais

A autenticação é JWT com HMAC-SHA256, emissor, audiência, validade de oito horas e tolerância de um minuto (MICROSOFT, [s.d.]a). Sem chave de pelo menos 32 caracteres a API não inicia. O login é anônimo. A conta de gestor vem da configuração e recebe o papel Gestor, sem identificador de aluno. O colaborador só entra se a senha de desenvolvimento conferir e se existir aluno com aquele e-mail; o token leva o claim `alunoId`. Matrícula e chamado exigem bearer. O gestor opera qualquer aluno. O colaborador que enviar outro identificador recebe 403.

As senhas e a chave em `appsettings` são valores de desenvolvimento, no mesmo espírito do placeholder já usado no banco local. Não são segredo de produção. No cluster, chave e cadeia de conexão saem do Secret `alunostech-api`. O `kubectl exec` na API devolveu `uid=1654(app)`. O papel do banco continua sem `DELETE`. O PostgreSQL do cluster não tem Service externo.

## 2.4 Manifestos aplicados neste cluster

O Deployment pede duas réplicas, atualização gradual com nenhuma indisponível e uma extra durante a troca, usuário e grupo 1654, pedido e limite de CPU e de memória. A prontidão consulta `GET /health/ready`. Essa rota testa a conexão com o PostgreSQL e fica Unhealthy se o banco não responder, de modo que o Service não mande tráfego a um pod sem persistência. A vida consulta `GET /health/live`, que só confirma o processo e não depende do banco, para não reiniciar a API numa falha transitória de rede (MICROSOFT, [s.d.]e). O HorizontalPodAutoscaler varia de duas a seis réplicas com alvo de 70% de CPU (KUBERNETES, [s.d.]). A imagem em `deploy/kubernetes/api-deployment.yaml` é `localhost:5001/alunos-tech-api:4604e61`.

O `kubectl get nodes` mostrou `alunos-tech-control-plane` Ready, Kubernetes v1.33.1. O Service nasceu como LoadBalancer e ficou com `EXTERNAL-IP` pending, porque o kind não entrega endereço de balanceador. O mecanismo que respondeu foi NodePort 30080, publicado pelo kind na porta 18080 do host. `GET http://127.0.0.1:18080/health/live` e `/health/ready` devolveram 200 e o corpo Healthy.

O metrics-server foi instalado com `--kubelet-insecure-tls`. O `kubectl top pods` listou CPU e memória dos pods da API e do banco. O HPA `alunostech-api` existe, com mínimo 2, máximo 6 e alvo de 70% de CPU. Na primeira leitura estável, o alvo apareceu como `cpu: 10%/70%` e a condição `ScalingActive` veio `ValidMetricFound`. Depois que o Docker Desktop voltou e os pods reiniciaram, a mesma leitura mostrou `cpu: 145%/70%` e o número de réplicas subiu a 6, que é o máximo do objeto. Não há Prometheus, Grafana nem regra de alerta instalados. A coleta que respondeu é o metrics-server, o `kubectl top` e o próprio HPA.

## 2.5 Leitura operacional do pipeline e da imagem

O job hospedado no GitHub Actions constrói a imagem e não a publica. Depois do checkout, configura o SDK 8, restaura o projeto da API — e, com ele, domínio, aplicação e infraestrutura —, compila em Release e executa `dotnet test` quando acha projeto de teste. O Buildx lê o Dockerfile da raiz com `push` falso. Esse job não envia a tag e não atualiza este cluster.

A entrega executada foi local. A imagem `localhost:5001/alunos-tech-api:4604e61` foi publicada no registro local e o `kubectl rollout` concluiu com as réplicas disponíveis. O job do GitHub não enxerga esse registro nem esse nó. O `push` falso descreve só o workflow hospedado, não a tag que o cluster puxou.

A imagem do cluster roda com o usuário 1654, na porta 8080, sem diagnóstico do runtime. Não há certificado nem ingresso. A sonda de prontidão é a condição para o pod receber pedido, e foi essa a rota que ficou Healthy antes de o Service entregar tráfego. O ambiente obtido é o cluster local automatizado na entrega da tag `4604e61`, com um único nó.

Uma requisição observada no cluster entra pelo host em `127.0.0.1:18080`. O kind entrega essa porta ao NodePort. A sonda `/health/ready` abre escopo, testa o PostgreSQL interno e responde Healthy. A sonda `/health/live` não consulta o banco. O aplicativo MAUI, no emulador, usou `adb reverse` dessa mesma porta e recebeu HTTP 201 na matrícula. O plano de controle que chamou as sondas foi o kubelet deste nó, não um conjunto de regiões.

## 2.6 Inventário do que está em execução

Convém fechar o capítulo com o inventário, para que o cluster local não seja lido como nuvem de provedor. O Compose sobe só o PostgreSQL. A imagem do cluster saiu de `Dockerfile.cluster`, foi enviada a `localhost:5001` e está aplicada na API. API e PostgreSQL rodam no kind, no nó `alunos-tech-control-plane`, Kubernetes v1.33.1. As sondas responderam Healthy. Uma leitura anterior do HPA chegou a seis réplicas com a CPU acima de 70% do pedido; a Figura 11 mostra a leitura posterior, com 2 réplicas e CPU em 2% de 70%.

Fora desta máquina continuam o registro público, o certificado TLS, o painel de métricas e qualquer balanceador de AWS, Azure ou Google. O endereço que funcionou é o NodePort do kind em 127.0.0.1:18080. O job do GitHub não enxerga esse cluster. A resiliência observada aqui é a de um nó, com troca de pods, sonda e HPA. Não é a de uma região de provedor.
