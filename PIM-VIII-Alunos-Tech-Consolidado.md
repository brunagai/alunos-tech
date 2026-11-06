# Capa

UNIVERSIDADE PAULISTA – UNIP EaD

Projeto Integrado Multidisciplinar

Curso Superior de Tecnologia em Análise e Desenvolvimento de Sistemas

[NOME COMPLETO DO AUTOR] – RA [NÚMERO DE MATRÍCULA]

**PLATAFORMA DE GESTÃO DE CURSOS E SUPORTE CORPORATIVO**

desenvolvimento de sistema para a empresa Alunos Tech

[CIDADE]

2026

# Folha de rosto

[NOME COMPLETO DO AUTOR] – RA [NÚMERO DE MATRÍCULA]

**PLATAFORMA DE GESTÃO DE CURSOS E SUPORTE CORPORATIVO**

desenvolvimento de sistema para a empresa Alunos Tech

Projeto Integrado Multidisciplinar para obtenção do título de tecnólogo em Análise e Desenvolvimento de Sistemas, apresentado à Universidade Paulista – UNIP EaD.

Orientador(a):

[CIDADE]

2026

# Resumo

O presente trabalho desenvolve a Plataforma de Gestão de Cursos e Suporte Corporativo da Alunos Tech, empresa fictícia de serviços corporativos digitais. O objetivo geral foi integrar capacitação e suporte com nuvem e DevOps, gestão ágil, programação .NET e programação de banco de dados. O problema reúne integração de sistemas, gerenciamento de informações, segurança dos dados, comunicação entre equipes e escalabilidade. A solução entrega uma API .NET 8 em camadas, um portal web do gestor e um cliente móvel do colaborador, ambos servidos pela API e autenticados por JWT. O gestor opera qualquer aluno; o colaborador opera apenas o próprio identificador. O PostgreSQL 16 persiste aluno, curso, matrícula e chamado, com chave estrangeira, índice único, procedimento de matrícula em lote, função, gatilho e papel de aplicação sem permissão de exclusão. O Docker Compose executa o banco. O pipeline constrói a imagem da API e não a publica. Os manifestos Kubernetes descrevem Deployment, Service e autoscaling e apontam para as sondas `/health/live` e `/health/ready`, sem aplicação em cluster. O módulo desktop permanece proposta. Testes de unidade aprovaram a matrícula válida, a ausência de aluno, a ausência de curso e a matrícula duplicada. Conclui-se que a plataforma integrada está operante no repositório e que a escala em cluster e o cliente desktop ficam como evolução.

**Palavras-chave:** Arquitetura de software. .NET. PostgreSQL. Docker. Teste de software.

# Abstract

This work develops the Course Management and Corporate Support Platform for Alunos Tech, a fictional digital corporate services company. The general objective was to integrate training and support with cloud and DevOps practices, agile project management, .NET programming, and database programming. The problem brings together system integration, information management, data security, communication between teams, and scalability. The solution delivers a layered .NET 8 API, a manager web portal, and a mobile employee client, both served by the API and authenticated with JWT. The manager operates on any employee; the employee operates only on the identifier carried by the token. PostgreSQL 16 stores the employee, the course, the enrollment, and the ticket, with foreign keys, a unique index, a batch enrollment procedure, a function, a trigger, and an application role that cannot delete rows. Docker Compose runs the database. The pipeline builds the API image and does not publish it. The Kubernetes manifests describe the Deployment, the Service, and autoscaling, and they call the `/health/live` and `/health/ready` probes, without being applied to a cluster. The desktop module remains a proposal. Unit tests passed valid enrollment, missing employee, missing course, and duplicate enrollment. It is concluded that the integrated platform is operational in the repository, while cluster scale and the desktop client remain future work.

**Keywords:** Software architecture. .NET. PostgreSQL. Docker. Software testing.

# 1. Introdução e contextualização

## 1.1 Apresentação da empresa

A Alunos Tech é uma organização fictícia do segmento de serviços corporativos digitais. Sua atuação consiste em apoiar empresas na capacitação de colaboradores e no atendimento de demandas técnicas internas, por meio de soluções de software sob medida. O público atendido é formado por gestores de equipes, analistas de desenvolvimento e demais profissionais que participam de trilhas de formação continuada no ambiente de trabalho.

No mercado corporativo digital, a empresa posiciona-se como provedora de uma plataforma única para dois fluxos que, com frequência, permanecem separados: a gestão de cursos e matrículas e o registro de chamados de suporte. A proposta não se restringe à disponibilização de conteúdo. Ela organiza o vínculo entre o colaborador, o curso e o chamado técnico, de modo que a área gestora acompanhe a capacitação e o atendimento a partir da mesma base de informações.

Para sustentar essa operação, a Alunos Tech adota um sistema denominado Plataforma de Gestão de Cursos e Suporte Corporativo. O sistema é desenvolvido em .NET 8, com persistência em PostgreSQL 16 e separação de responsabilidades segundo a Clean Architecture. Essa escolha técnica reflete a necessidade de um produto evolutivo, testável e passível de manutenção por equipes de Análise e Desenvolvimento de Sistemas.

## 1.2 Definição do problema de negócios

Organizações que capacitam colaboradores e, ao mesmo tempo, atendem incidentes técnicos costumam distribuir essas atividades em planilhas, caixas de correio e ferramentas sem integração. O registro da matrícula em um curso não conversa com o histórico de chamados do mesmo colaborador. Na Alunos Tech, esse desencontro aparece em cinco dificuldades do enunciado, tratadas de forma concreta pela plataforma.

A integração de sistemas deixa de depender de exportação manual porque o portal do gestor e o cliente móvel do colaborador chamam a mesma API. O gerenciamento de informações concentra aluno, curso, matrícula e chamado em PostgreSQL, com chave estrangeira e índice único. A segurança dos dados combina token JWT, papéis de gestor e de colaborador e um papel de banco sem permissão de exclusão. A comunicação entre equipes passa pelo backlog, pelas Sprints e pelo chamado formal, com título, prioridade e status, em lugar de mensagem solta. A escalabilidade aparece no contêiner da API, nas sondas de saúde e nos manifestos Kubernetes, com o limite honesto de que o cluster ainda não está em execução.

O problema de negócio consiste, portanto, em centralizar capacitação e suporte sem perder rastreabilidade, controle de acesso e possibilidade de crescer além de uma única máquina de desenvolvimento.

## 1.3 Objetivos do projeto

### 1.3.1 Objetivo geral

Desenvolver uma solução tecnológica corporativa integrada para a Alunos Tech, reunindo Cloud Computing e DevOps, gerenciamento ágil de projetos, programação aplicada em .NET e programação de banco de dados na Plataforma de Gestão de Cursos e Suporte Corporativo.

### 1.3.2 Objetivos específicos

1. Desenvolver aplicações web e móvel integradas à mesma API de matrícula e de chamado.
2. Estruturar um banco relacional seguro e otimizado, com procedimento, função, gatilho, transação, índice e controle de concorrência.
3. Aplicar práticas de nuvem e DevOps no que o repositório executa e registrar, em manifesto, o que depende de cluster.
4. Conduzir o projeto com Scrum e Kanban, de modo que a comunicação entre as frentes fique rastreável.
5. Implementar controles de segurança e de capacidade já utilizáveis: autenticação, autorização, papel mínimo no banco, sondas de saúde e imagem construída pelo pipeline.
6. Propor a arquitetura alinhada ao negócio e, no eixo .NET, propor o módulo desktop sem apresentá-lo como construído.
7. Aplicar orientação a objetos em C#, estrutura modular, padrões de projeto, testes e critérios de manutenção.

## 1.4 Justificativa

Do ponto de vista técnico, a fragmentação entre capacitação e suporte justifica um sistema com modelo relacional explícito. O colaborador relaciona-se com várias matrículas e com vários chamados; o curso relaciona-se com várias matrículas; e a combinação de colaborador e curso deve ocorrer uma única vez. Essas regras são próprias de um banco relacional e de um domínio orientado a objetos, e não se sustentam de modo seguro em controles manuais.

A arquitetura adotada separa a regra de negócio da tecnologia de persistência e da interface HTTP. O domínio valida nome, e-mail, carga horária, prioridade e duplicidade de matrícula. A aplicação orquestra os repositórios. A infraestrutura implementa o acesso via Entity Framework Core e o provedor Npgsql. A API apenas traduz a requisição e o resultado. Essa divisão reduz o acoplamento e permite que cada camada seja compreendida, testada e evoluída com responsabilidade definida, prática central da Engenharia de Software.

A persistência em PostgreSQL 16 acrescenta mecanismos que o modelo orientado a objetos, sozinho, não oferece no banco: chaves estrangeiras, índice único composto em matrículas, procedimento de matrícula em lote e gatilho de atualização da data de alteração do chamado. O ambiente em contêiner e o pipeline de build, teste e imagem Docker aproximam o projeto de um fluxo profissional de entrega, ainda que a publicação em produção permaneça simulada nesta etapa.

Do ponto de vista acadêmico, o trabalho integra conteúdos próprios de Análise e Desenvolvimento de Sistemas: levantamento de requisitos, modelagem de domínio, projeto de banco de dados, programação orientada a objetos, arquitetura de software, desenvolvimento de API, testes automatizados e práticas de DevOps. A empresa fictícia delimita um contexto verossímil, sem depender de dados reais de terceiros, e ainda assim impõe restrições de integridade, rastreabilidade e manutenção comparáveis às de um sistema corporativo.

A relevância do projeto está, portanto, na resolução de um problema de gestão — centralizar capacitação e suporte — por meio de um software estruturado, verificável e documentado. O resultado esperado não é apenas a implementação das funcionalidades de matrícula e de chamado, mas a demonstração de que decisões de arquitetura, de dados e de qualidade podem ser justificadas e sustentadas ao longo do ciclo de desenvolvimento.

## 1.5 Metodologia utilizada

O trabalho adota pesquisa aplicada, de natureza tecnológica. O problema parte de uma necessidade concreta da Alunos Tech — centralizar capacitação e suporte — e o resultado é um artefato de software verificável, acompanhado da justificativa das decisões de projeto. A investigação produz conhecimento na medida em que registra como regras de matrícula, de chamado e de persistência foram traduzidas em uma solution executável.

O desenvolvimento apoia-se na engenharia de requisitos. O domínio foi delimitado em colaborador, curso, matrícula e chamado de suporte. De cada elemento foram extraídas restrições observáveis: identificadores obrigatórios, carga horária positiva, prioridade pertencente a um conjunto finito, vínculo obrigatório com o colaborador e unicidade do par colaborador-curso. Essas restrições tornaram-se critérios de aceite das histórias de usuário e, em seguida, validação no domínio, contrato dos casos de uso e restrição no PostgreSQL. O requisito deixa de ser uma descrição solta e passa a ser uma condição que o incremento precisa satisfazer.

A condução do projeto segue práticas ágeis. O Scrum organiza o trabalho em ciclos com objetivo, incremento e revisão (SCHWABER; SUTHERLAND, 2020). O Product Backlog ordena épicos e histórias da Alunos Tech. Quatro Sprints separam infraestrutura e domínio, casos de uso e repositórios, banco avançado e API, qualidade e integração contínua. Um quadro Kanban torna visível o estado de cada história dentro da Sprint e limita o volume de trabalho simultâneo. O detalhamento desses artefatos, dos eventos e da Definition of Done ocupa o capítulo de gerenciamento ágil.

A construção técnica fundamenta-se na arquitetura limpa, nos testes automatizados e na modelagem relacional. A separação entre domínio, aplicação, infraestrutura e interface segue a regra de dependência descrita por Martin (2019): a regra de negócio não referencia o banco nem o HTTP. A verificação da matrícula usa testes de unidade com substitutos dos repositórios, de modo que a regra seja exercida antes da persistência. O estado durável é um modelo relacional em PostgreSQL 16, com chave primária, chave estrangeira, índice e programação no próprio banco, conforme a documentação do SGBD (POSTGRESQL GLOBAL DEVELOPMENT GROUP, 2026). A plataforma .NET 8, o Entity Framework Core e o comportamento da API apoiam-se nas documentações oficiais da Microsoft (2023c, 2023d). O capítulo de engenharia e o de persistência desenvolvem cada uma dessas escolhas. A metodologia, neste ponto, apenas fixa o caminho: requisito explícito, entrega incremental e fundamento técnico citado.

# 2. Arquitetura de nuvem e DevOps

## 2.1 Arquitetura alinhada à integração

A dificuldade de integração de sistemas, na Alunos Tech, era a matrícula viver numa planilha e o chamado viver noutra caixa de entrada. A arquitetura que responde a isso é uma API REST única, em .NET 8, consumida pelo portal do gestor e pelo cliente móvel do colaborador. Os dois clientes não têm banco próprio e não repetem a regra de duplicidade. Quem integra os fluxos é o contrato HTTP: login, matrícula e chamado. O domínio, a aplicação, a infraestrutura e a interface permanecem em projetos separados, com a regra de negócio independente do PostgreSQL e do HTTP (MARTIN, 2019).

Essa escolha é deliberadamente modular e não é uma malha de microsserviços. Não há repositório de serviços autônomos, nem barramento, nem deploy separado de “serviço de matrícula” e “serviço de chamado”. Há uma aplicação que o negócio reconhece como a plataforma, publicada como uma imagem. A topologia de nuvem prevista coloca essa imagem atrás de um Service, com o banco fora do pod e com Secret para a cadeia de conexão e para a chave do token. O desenho está em `deploy/kubernetes`. Ele descreve o alvo. Não foi aplicado a cluster.

O gerenciamento de informações fica no PostgreSQL 16, sistema de registro de aluno, curso, matrícula e chamado. A escalabilidade pretendida não nasce de duplicar planilha: nasce de réplicas da API sem estado, de sondas que só recebem tráfego quando o banco responde e de um autoscaling escrito no manifesto. O que já roda é mais estreito e está declarado na seção seguinte.

## 2.2 Ambiente que executa hoje

O Dockerfile parte da imagem de SDK 8.0.427, copia os quatro arquivos de projeto necessários ao restore e só então copia o código. A publicação gera a API, incluindo as páginas de `wwwroot`. A imagem final escuta a porta 8080, usa o usuário 1654 e desliga o diagnóstico do runtime (DOCKER, 2026b). O caminho do projeto no argumento é `src/Api/AlunosTech.Api/AlunosTech.Api.csproj`, o mesmo usado pelo pipeline. Um caminho anterior, sem a pasta `Api`, quebraria o restore.

O Docker Compose sobe somente o PostgreSQL 16 Alpine. Há volume nomeado, rede do projeto, porta presa a 127.0.0.1 e verificação `pg_isready`. A API não é serviço do Compose. Subir o banco não publica a plataforma (DOCKER, 2026a). A API local conecta com o papel `alunos_tech_app`. O superusuário fica para a administração do schema.

O fluxo de integração no GitHub Actions dispara no push da branch principal. Restaura e compila a API em Release, executa a suíte quando acha projeto de teste e constrói a imagem com Buildx, com `push` falso. A tag usa o identificador do commit. O job seguinte escreve o plano de registro e de atualização. Não executa login, push nem contato com host. Não há entrega contínua aplicada. Há integração contínua capaz de produzir a imagem. Afirmar deploy seria simular uma máquina que este trabalho não possui.

## 2.3 Segurança da API e das credenciais

A autenticação é JWT com HMAC-SHA256, emissor, audiência, validade de oito horas e tolerância de um minuto (MICROSOFT, 2023a). Sem chave de pelo menos 32 caracteres a API não inicia. O login é anônimo. A conta de gestor vem da configuração e recebe o papel Gestor, sem identificador de aluno. O colaborador só entra se a senha de desenvolvimento conferir e se existir aluno com aquele e-mail; o token leva o claim `alunoId`. Matrícula e chamado exigem bearer. O gestor opera qualquer aluno. O colaborador que enviar outro identificador recebe 403.

As senhas e a chave em `appsettings` são valores de desenvolvimento, no mesmo espírito do placeholder já usado no banco local. Não são segredo de produção. No manifesto, chave e cadeia de conexão saem de um Secret nomeado `alunostech-api`. Esse Secret não foi criado em cluster, porque o cluster não foi criado. A segurança que está em uso é a da API local e a do papel de banco sem `DELETE`. A segurança desenhada para a nuvem acrescenta o Secret e o usuário não root já presente na imagem.

## 2.4 Manifestos, sondas e o que não sobe

O Deployment pede duas réplicas, atualização gradual com nenhuma indisponível e uma extra durante a troca, usuário e grupo 1654, pedido e limite de CPU e de memória. A prontidão consulta `GET /health/ready`. Essa rota testa a conexão com o PostgreSQL e fica Unhealthy se o banco não responder, de modo que o Service não mande tráfego a um pod sem persistência. A vida consulta `GET /health/live`, que só confirma o processo e não depende do banco, para não reiniciar a API numa falha transitória de rede (MICROSOFT, 2023e). O Service é LoadBalancer, da porta 80 para a 8080. O HorizontalPodAutoscaler varia de duas a seis réplicas com alvo de 70% de CPU (KUBERNETES, 2026). A imagem do manifesto usa um marcador de commit, não um endereço de registro real.

Nenhum `kubectl apply` foi executado. Não há kubeconfig, nó nem balanceador neste trabalho. A resiliência já obtida é a do volume do PostgreSQL no Compose, a da imagem reproduzível e a do índice único, que segura matrícula duplicada quando duas requisições passam juntas. A resiliência do Deployment — troca gradual, sonda e HPA — está escrita e não foi medida.

O monitoramento implementado é esse par de verificações. Não há Prometheus, painel nem alerta no repositório. Métricas, log central e TLS no ingresso ficam como evolução, junto da publicação da imagem. O capítulo separa, de propósito, o pipeline que constrói, o Compose que sobe o banco e o cluster que ainda é manifesto.

## 2.5 Leitura operacional do pipeline e da imagem

O job de integração não trata a imagem como efeito colateral. Depois do checkout, configura o SDK 8, restaura o projeto da API — e, com ele, domínio, aplicação e infraestrutura —, compila em Release sem restaurar de novo e procura um projeto de teste pelo nome. Se encontra, executa `dotnet test`. Em seguida o Buildx lê o Dockerfile na raiz e passa o mesmo caminho de projeto usado na compilação. A imagem recebe a tag do commit e permanece na máquina do workflow. Nada é enviado.

Essa sequência é o DevOps aplicado ao estágio atual. Ela garante que a solution que os testes aprovam é a solution que a imagem empacota, inclusive as páginas do gestor e do colaborador. Ela não garante que um cluster puxou essa tag. O job de entrega escreve o registro previsto, o nome da imagem e o fato de o segredo de produção estar ou não configurado, sem imprimir o segredo. Os comandos de login e de push permanecem comentados. A automação, portanto, cobre a construção. A automação de publicação está reservada e visível, para não ser confundida com uma execução.

A segurança dessa imagem está no usuário não privilegiado, na porta explícita e na ausência de ferramenta de diagnóstico. A segurança do tráfego, com certificado e ingresso, não está no Dockerfile e não deve ser inferida dele. Quando o cluster existir, o Secret substitui a chave de desenvolvimento e a sonda de prontidão continua sendo a condição para receber pedido. Até lá, o ambiente cloud seguro e automatizado que o manual descreve como resultado está parcialmente realizado: automatizado na construção, seguro na API e no papel do banco, e ainda não resiliente em nós distribuídos.

Uma requisição que já pode ser observada percorre outro caminho, sem cluster. O navegador pede a página em `/gestor/` ou `/colaborador/`. A API devolve o arquivo estático. O login segue para `/api/auth/login`. Se a credencial confere, o cliente guarda o token só na memória da página e o reenvia em matrícula ou chamado. A sonda `/health/ready`, nesse mesmo processo, abre um escopo, pede ao contexto se o PostgreSQL aceita conexão e responde saudável ou não. É essa rota, e não uma URL fictícia, que o manifesto de prontidão nomeia. A rota `/health/live` não consulta o banco. Separar as duas evita que uma falha de persistência reinicie o processo, e evita que um processo sem banco receba matrícula. Esse comportamento está no código. O que falta é o plano de controle que chamaria essas rotas em vários nós.

## 2.6 Inventário do que está em execução

Convém fechar o capítulo com o inventário, para que a arquitetura desenhada não seja lida como ambiente já distribuído. Em execução local estão o contêiner do PostgreSQL 16, saudável por `pg_isready`, e a API em .NET 8, com JWT, com as duas sondas e com as páginas do gestor e do colaborador. O papel `alunos_tech_app` é o usuário dessa API. A imagem da API é produzida pelo Dockerfile de dois estágios e pelo job de integração, que compila, testa e constrói sem enviar a tag a um registro. Os manifestos em `deploy/kubernetes` nomeiam as sondas reais, o usuário 1654, o Service e o autoscaling de duas a seis réplicas.

Fora de execução estão o push da imagem, o Secret no cluster, o `kubectl apply`, o balanceador, o HPA medido, o certificado TLS e qualquer painel de métricas. O job de entrega escreve esse plano e não o cumpre. Tratar o LoadBalancer como endereço público deste PIM seria simular uma máquina que não existe. O resultado de nuvem, no estágio entregue, é portanto um ambiente reproduzível e com controles de acesso já aplicados, mais um conjunto de manifestos prontos para um cluster futuro. A resiliência distribuída continua sendo o próximo incremento, não uma propriedade atual da plataforma.

# 3. Gerenciamento ágil do projeto

## 3.1 Método e comunicação

A comunicação entre as frentes da Alunos Tech — domínio, banco, API e clientes — foi tratada como problema de gestão, não só como problema de código. O Scrum organiza objetivo, incremento e revisão (SCHWABER; SUTHERLAND, 2020). O Kanban mostra a coluna de cada história e limita o que pode estar em andamento. O critério de pronto exige artefato utilizável: regra no domínio ou no banco, endpoint ou script, e verificação executada. Texto que anuncia um recurso sem o arquivo correspondente não fecha a história. O Manifesto Ágil sustenta a preferência por software funcionando e por resposta à mudança (BECK *et al.*, 2001).

Não se inventa aqui uma equipe com papéis ocupados por pessoas que não atuaram. O backlog é o quadro da frente técnica deste PIM. A revisão compara o objetivo do ciclo com o teste, com o script ou com a tela que chama a API. Essa disciplina evita que a integração entre matrícula e chamado volte a ser combinada por recado.

## 3.2 Backlog e histórias

O épico de alunos pede cadastro com nome, e-mail, cargo opcional e situação ativa, porque o login do colaborador procura o e-mail nessa tabela. O épico de cursos pede título, descrição opcional, carga horária positiva e situação ativa. O épico de matrículas pede a inclusão de um par aluno-curso, a recusa de identificador vazio, a recusa de aluno ou curso inexistente e a recusa de duplicidade. A história de lote pede um procedimento que grave a linha inteira e ignore o par já existente. O épico de chamados pede título, descrição, prioridade num conjunto finito e status, sempre ligados a um aluno. A habilitação pede contêiner do banco, migração, testes, pipeline, autenticação, clientes e manifestos.

A ordem seguiu a dependência. Sem aluno e curso não há matrícula. Sem caso de uso não há endpoint. Sem endpoint autenticado o cliente web seria uma proposta. Cada história carrega o critério que a Sprint usa na revisão, e não uma estimativa em pontos que este trabalho não mediu.

## 3.3 Ciclos de entrega

A Sprint 1 teve por objetivo deixar o ambiente e o domínio reproduzíveis. Entregou o Compose do PostgreSQL, a solution em quatro projetos de produção mais o de teste, e as entidades com estado protegido. A revisão conferiu o contêiner saudável e a compilação da solution.

A Sprint 2 teve por objetivo isolar a regra da interface. Entregou repositórios, DTOs e os casos de uso de matrícula e de chamado. A matrícula valida identificadores, existência e duplicidade. O chamado valida a prioridade. A revisão ficou nos testes de unidade da matrícula, escritos no ciclo seguinte e já previstos como prova dessa regra.

A Sprint 3 teve por objetivo persistir e expor. Entregou as migrations, o script com índices, procedimento, função e gatilho, e os endpoints de matrícula e de chamado, com falha de domínio em HTTP 400. A revisão executou o procedimento dentro de uma transação desfeita, para não deixar dado de ensaio.

A Sprint 4 teve por objetivo verificar e construir a imagem. Entregou quatro testes aprovados e o pipeline que compila e gera a imagem sem publicá-la. A revisão do manual, feita depois, reabriu o backlog: a procedure não preenchia as colunas obrigatórias, o pipeline apontava para um caminho antigo, e JWT, sondas, web e móvel existiam só no texto.

O incremento de adequação não foi renomeado como Sprint pretérita. Ele entregou o login, a autorização, as rotas de saúde, os dois clientes, o `INSERT` completo, o papel de banco e os manifestos. O desktop e a subida do cluster permaneceram fora do pronto, com registro explícito de proposta e de manifesto.

## 3.4 Acompanhamento e melhoria

O acompanhamento não usa percentual de conclusão. Usa a coluna do quadro e o resultado do comando. História em revisão só sai quando o teste passa, o script aplica ou a página chama o endpoint. A retrospectiva deste PIM fixou três correções de processo. A primeira: o capítulo não pode usar o verbo implementar para arquivo inexistente. A segunda: script SQL precisa ser ensaiado no esquema da migration, não num esquema imaginado com valor padrão. A terceira: pipeline e Dockerfile precisam apontar para o mesmo projeto, senão a imagem que o texto chama de construída não constrói.

Essas correções são o conteúdo ágil aplicado à mudança de requisito. O manual exigiu web, móvel, Kubernetes utilizável por sondas reais e pipeline que gera imagem. O backlog absorveu o acréscimo, o incremento entregou o mínimo honesto e a documentação passou a distinguir execução de desenho. É esse o mecanismo de comunicação que substitui a ferramenta solta: um item, um critério e um artefato no repositório.

## 3.5 Quadro e definição de pronto

O quadro usado tem quatro colunas: a fazer, em andamento, em revisão e pronto. O limite de em andamento evita abrir o cliente móvel antes de existir autorização, ou escrever o manifesto antes de existir a rota de saúde. Uma história só entra em revisão com o caminho do arquivo indicado. Só entra em pronto com a verificação correspondente: compilação, teste, script aplicado ou página que chama o verbo.

A definição de pronto da matrícula ilustra o critério. Não basta o caso de uso devolver um identificador no teste. É preciso o endpoint exigir token, o banco recusar o par duplicado e o procedimento, no lote, preencher as colunas que a migration deixou sem padrão. A definição de pronto do chamado ilustra o outro fluxo: prioridade válida, aluno existente e gatilho capaz de renovar `UpdatedAt` sem o cliente enviar a data. A definição de pronto da nuvem é a mais restrita. Compose do banco, imagem construída sem push e manifesto com as sondas reais estão prontos. Cluster aplicado não entra na coluna, porque não há evidência.

O planejamento de cada Sprint partiu dessas definições, não de uma velocidade inventada. A revisão olhou o incremento. A retrospectiva devolveu ao quadro o que a evidência não sustentava. Esse ciclo é o uso da metodologia ágil neste PIM: poucas cerimônias, critério observável e mudança de escopo registrada quando o manual exigiu mais do que a proposta inicial continha.

A história do cliente móvel mostra o critério em uso. Ela só saiu de “a fazer” quando o login do colaborador já devolvia `alunoId` e quando a matrícula recusava outro aluno. Entrou em revisão com a página em `/colaborador/` chamando os três verbos. Não foi dada como pronta por um desenho de tela. A história do manifesto teve critério diferente e mais estreito: os arquivos existem, as sondas apontam para rotas que respondem, e nenhum comando de aplicação em cluster entra no pronto. Registrar as duas histórias lado a lado é o que permite à equipe — ainda que a frente seja a deste trabalho — falar da mesma entrega sem chamar de publicado o que está apenas descrito. A comunicação deixa de ser a ferramenta solta do enunciado e passa a ser o quadro.

O limite do quadro também protege o prazo. História sem evidência não avança para abrir outra frente. Por isso o desktop não ocupou a coluna de em andamento ao mesmo tempo que a correção da procedure: os dois competiriam pela mesma revisão e o texto tenderia a antecipar a tela. A procedure voltou ao pronto só depois do `CALL` no esquema real, com a segunda chamada inserindo zero linhas e com o `ROLLBACK` limpando o ensaio. O desktop ficou em a fazer, com a arquitetura escrita e sem projeto. Esse corte é a melhoria contínua aplicada a um PIM: menos itens simultâneos e nenhum item promovido por expectativa.

# 4. Engenharia de software e programação em .NET

## 4.1 Objetos, SOLID e módulos

O domínio representa aluno, curso, matrícula e chamado com propriedades de escrita privada. Nome, e-mail, carga horária e prioridade são validados na construção. A falha de negócio é `DomainException`, não um código HTTP (MICROSOFT, 2023b). A plataforma é .NET 8 (MICROSOFT, 2023d). A dependência aponta para dentro: o domínio não referencia aplicação, infraestrutura nem ASP.NET Core (MARTIN, 2019).

A responsabilidade única aparece na divisão da solution `AlunosTech.sln`. `AlunosTech.Domain` guarda entidade, enumeração e exceção. `AlunosTech.Application` guarda DTO, contrato de repositório e caso de uso. `AlunosTech.Infrastructure` guarda o contexto, o mapeamento Fluent, a migration e o repositório. `AlunosTech.Api` guarda controller, filtro, token, sondas e páginas. `AlunosTech.Tests` referencia a aplicação e substitui o repositório. Aberto para extensão, o caso de uso recebe interface; fechado para alteração casual, a regra de duplicidade não mora no controller. A substituição de princípio fica no teste, que não precisa do PostgreSQL. A segregação evita um repositório genérico exposto ao cliente HTTP. A inversão está no registro de injeção: o controller pede o caso de uso, e o caso de uso pede a interface.

## 4.2 Padrões, fluxo e desempenho

O padrão Repository isola a consulta. `AddAsync` chama `SaveChangesAsync` na hora, então não há classe Unit of Work. Cada inclusão é uma transação do Entity Framework. O DTO atravessa a borda HTTP sem expor a entidade. A injeção registra infraestrutura, os dois casos de uso e o emissor de token como singleton. O filtro `DomainExceptionFilter`, registrado nos controllers, devolve 400 com a mensagem de domínio. Não é middleware. `DbUpdateException`, típica do índice único quando duas requisições ganham a corrida, não é convertida em 400.

O fluxo da matrícula é curto. O controller exige o token e confere o papel. O caso de uso recusa identificador vazio, aluno ausente, curso ausente e par já existente, e só então cria a matrícula ativa. O chamado recusa prioridade fora do conjunto e aluno ausente. A leitura dessas checagens percorre a coleção em memória. Isso é simples e é limitado: a lista cresce com a tabela. A garantia sob concorrência não é essa leitura. É o índice único do par aluno-curso, descrito no capítulo de banco. A manutenção futura pode trocar a leitura por consulta filtrada sem mudar o cliente, porque o contrato do endpoint permanece o identificador devolvido.

Os testes de unidade cobrem matrícula válida, aluno inexistente, curso inexistente e matrícula repetida, com Moq e FluentAssertions. A suíte aprovou os quatro fatos. Não há teste de carga, teste do filtro HTTP nem teste de tela. A manutenção se apoia na compilação separada dos projetos e nesse núcleo de regra protegido. O que não está coberto permanece dívida visível, não qualidade implícita.

## 4.3 Módulo desktop proposto

O eixo de programação aplicada pede propor o módulo desktop e a arquitetura, não afirmar que a tela foi construída. A proposta é um projeto à parte na mesma solution, sem referência à infraestrutura e sem cadeia de conexão. Ele chamaria `POST /api/auth/login`, `POST /api/matriculas` e `POST /api/chamados` com o token do gestor, e uma operação futura de lote apontaria para o procedimento já existente, por um endpoint que este repositório ainda não tem. A fila de chamados e a matrícula em lote ficariam nessa tela. A regra continuaria no domínio e no banco.

Não há projeto WinForms, WPF ou MAUI, nem binário desktop. Tratar essa proposta como módulo entregue contradiria o código. A arquitetura proposta, porém, já está alinhada ao que web e móvel fazem: um cliente, um token e a mesma API.

## 4.4 O que a manutenção precisa preservar

A manutenção do núcleo depende de três invariantes. A primeira é a escrita privada do estado: outro projeto não altera o status da matrícula por atribuição solta. A segunda é a exceção de domínio carregando a regra, e o filtro traduzindo só essa exceção. A terceira é o teste do caso de uso sem banco, para que uma mudança de mensagem ou de ordem de validação quebre a suíte antes de quebrar o portal. Quebrar uma dessas invariantes exige revisão, não um ajuste só na página.

O desempenho aceitável para o escopo é o da gravação pontual. Não se promete tempo de resposta sob centenas de usuários, porque não houve ensaio de carga. O ponto já conhecido de custo é a leitura integral na checagem de duplicidade e no login do colaborador. A correção natural, quando a tabela crescer, é uma consulta por e-mail e uma consulta pelo par aluno-curso, mantendo o índice único como última barreira. Antecipar essa consulta no texto sem tê-la no código repetiria o erro que este trabalho corrige.

O módulo desktop, se for construído depois, entra como mais um adaptador. Ele não recebe permissão de apagar linha, porque o papel da API também não recebe. Ele não valida duplicidade sozinho, porque dois clientes divergiriam. A estrutura modular já deixa esse encaixe: novo projeto, mesma aplicação, nenhuma referência ao contexto do Entity Framework.

A suíte atual protege quatro decisões, não a interface. No sucesso, aluno e curso existem, o par é novo e o caso de uso devolve o identificador da matrícula criada. Na ausência de aluno, a exceção ocorre antes de gravar. Na ausência de curso, o mesmo. Na matrícula repetida, a leitura encontra o par e a gravação não é chamada. Os repositórios são substitutos. O teste não abre PostgreSQL e não pede token: autenticação e índice único ficam fora dessa suíte, de propósito, e por isso não são tratados como cobertos por ela. A manutenção dessa fronteira importa. Um teste que passasse a exigir o banco deixaria de ser a prova rápida da regra e passaria a ser um ensaio de integração, que este repositório ainda não possui.

# 5. Persistência e programação em banco de dados

## 5.1 Modelo seguro e consultas

O gerenciamento de informações da Alunos Tech cabe em quatro tabelas. `Alunos` guarda identificador, nome, e-mail, cargo opcional, situação e carimbos. `Cursos` guarda título, descrição opcional, carga horária e situação. `Matriculas` guarda aluno, curso, status, data da matrícula e carimbos. `ChamadosSuporte` guarda aluno, título, descrição, status, prioridade e carimbos. As chaves primárias são `uuid`. As estrangeiras de matrícula e de chamado usam `ON DELETE RESTRICT`: o banco recusa apagar aluno ou curso ainda referenciado (POSTGRESQL GLOBAL DEVELOPMENT GROUP, 2026). O mapeamento é Fluent API pelo Npgsql, nas migrations `InitialCreate` e `AddEntitiesAndMappings` (MICROSOFT, 2023c; NPGSQL, 2026). Não há valor padrão de banco para o identificador da matrícula, para o status nem para as datas. Quem insere precisa preencher a linha.

A otimização está nos índices, não numa reescrita especulativa de consulta. Há índice em `Matriculas.AlunoId`, em `Matriculas.CursoId` e em `ChamadosSuporte.AlunoId`, para o filtro e a junção por um dos lados. O índice único `UX_Matriculas_AlunoId_CursoId` é ao mesmo tempo regra de negócio e controle de concorrência. Duas transações podem não ver a matrícula uma da outra e tentar gravar o mesmo par; a segunda falha no único, ou é ignorada quando o comando usa `ON CONFLICT`.

A segurança do banco não se esgota nesse índice. O script cria o papel `alunos_tech_app`, com uso do schema, `SELECT`, `INSERT` e `UPDATE` nas quatro tabelas, sem `DELETE`, e com execução de `sp_MatricularAlunoEmLote`. A API local conecta com esse papel. Um ensaio de exclusão foi negado pelo PostgreSQL e a transação foi desfeita. O papel `postgres` continua reservado à administração do schema e às migrations. Senha de desenvolvimento do papel de aplicação está no script, como placeholder, e deve ser trocada fora do repositório antes de qualquer ambiente compartilhado.

## 5.2 Programação no banco

`sp_MatricularAlunoEmLote(uuid, uuid[])` recusa aluno nulo e lista sem curso. O `INSERT` informa `Id`, `AlunoId`, `CursoId`, `Status` 1, que é a matrícula ativa, `DataMatricula`, `CreatedAt` e `UpdatedAt`. O identificador vem de `gen_random_uuid()` e as três datas de `NOW()`. Cursos repetidos na mesma chamada caem num `DISTINCT`. O `NOT EXISTS` evita o par já gravado, e `ON CONFLICT DO NOTHING` cobre a corrida. `GET DIAGNOSTICS` conta as linhas inseridas. Não há `COMMIT` dentro do procedimento: a transação pertence a quem chama, como recomenda o próprio SGBD para rotina que precisa poder ser desfeita.

A função `fn_chamados_suporte_set_updated_at` grava `NOW()` em `UpdatedAt` e devolve `NEW`. O gatilho `tr_chamados_suporte_set_updated_at` a dispara antes da atualização de cada chamado. Assim o carimbo não depende de a aplicação lembrar o campo.

No PostgreSQL local, o script foi aplicado de fato. Uma transação de ensaio criou aluno e dois cursos, chamou o procedimento e obteve duas matrículas com status, identificador e data preenchidos. A segunda chamada do mesmo par inseriu zero linhas. Um `UPDATE` de chamado fez o gatilho renovar `UpdatedAt`. O `ROLLBACK` encerrou o ensaio. O comportamento descrito é o que o banco executou, não uma leitura do script isolada.

## 5.3 Transação na API e concorrência

O caso de uso grava uma matrícula por `SaveChangesAsync`. Essa gravação é uma transação. A leitura anterior, que procura duplicidade na coleção, é outra operação. Entre as duas, outra requisição pode inserir o mesmo par. O índice único é o controle dessa corrida na API. O procedimento, no lote, usa a cláusula de conflito dentro do próprio comando. Os dois caminhos protegem a mesma regra por mecanismos diferentes, e o capítulo não os apresenta como se fossem um só.

Não há nível de isolamento customizado nem bloqueio consultivo. O controle de concorrência entregue é o índice único, o `ON CONFLICT` e a transação de cada gravação. Isso basta para a matrícula duplicada. Não basta para um relatório analítico sob carga, que este trabalho não implementou.

## 5.4 O que cada rotina garante

O procedimento garante a matrícula em lote sem depender de padrão de coluna. Se a lista trouxer o mesmo curso duas vezes, uma linha basta. Se o par já existe, a chamada termina sem erro e com contagem zero, o que permite repetir a operação. Se o aluno vem nulo, a rotina falha antes de gravar. Ela não cria aluno nem curso: a chave estrangeira continua dona dessa integridade. Por isso o ensaio precisou inserir os pais antes do `CALL`.

A função não contém regra de prioridade nem de status. Ela só impede que uma atualização de chamado deixe o carimbo antigo. O gatilho amarra essa função à tabela certa e ao momento certo, antes da escrita. Uma alteração futura de título passa pela função mesmo que o cliente esqueça o campo. Isso é auditoria mínima, não trilha histórica de versões. Não há tabela de log.

A transação do ensaio mostra o terceiro elemento. Como o procedimento não confirma sozinho, o `ROLLBACK` desfez aluno, curso, matrícula e chamado de teste. A mesma propriedade serve à aplicação: quem chamar o lote pode juntá-lo a outra escrita e desfazer o conjunto. A API, hoje, não chama o procedimento. Ela grava uma matrícula pelo repositório. Os dois mecanismos coexistem sem se substituir. Documentar o lote como se a tela já o disparasse seria antecipar o endpoint que a proposta desktop ainda espera.

O papel de aplicação fecha o mesmo desenho pelo privilégio, não pela convenção. Ele pode ler e gravar as quatro tabelas e executar o lote. Não pode apagar. O ensaio confirmou a negativa e não deixou a linha de teste. A API de desenvolvimento já usa esse usuário na cadeia de conexão. As migrations continuam a cargo do administrador, porque criar tabela e índice não é papel do runtime. Assim, uma falha no portal não ganha o direito de esvaziar `Matriculas` ou `Alunos`. O índice único continua necessário: impedir a segunda matrícula é regra de negócio, e o papel mínimo não a expressa. Os dois controles se completam. Um sem o outro deixaria ou a duplicidade ou a exclusão acidental sem resposta no banco.

A otimização permanece proporcional ao que a plataforma consulta. O filtro por aluno, na matrícula e no chamado, usa o índice da chave estrangeira. O par aluno-curso usa o único. Não se criou índice extra sobre título ou descrição, porque não há busca textual no caso de uso. Criá-lo agora seria aparência de desempenho, sem consulta que o aproveitasse. Quando existir listagem, o índice deve nascer com ela. Até lá, o plano de execução relevante é o da inclusão: a unicidade é verificada pelo índice, e o gatilho só escreve o carimbo da linha que já será atualizada. O custo fica ao lado da escrita que o negócio de fato faz.

# 6. Clientes web e móvel

## 6.1 Integração pela mesma API

Desenvolver aplicações web e móvel, no enunciado, não se cumpre com um desenho de tela. Os dois clientes estão em `wwwroot` e são servidos pela API, na mesma origem, junto com os arquivos estáticos. Não há política de CORS porque o navegador fala com o host que entregou a página. O portal do gestor responde em `/gestor/`. O cliente do colaborador responde em `/colaborador/`, com viewport móvel e controles altos o bastante para toque. Ambos usam somente três operações já existentes: `POST /api/auth/login`, `POST /api/matriculas` e `POST /api/chamados`. O corpo da matrícula leva aluno e curso. O corpo do chamado leva aluno, título, descrição e prioridade. A resposta de sucesso devolve o identificador. A falha de domínio volta 400. A credencial recusada volta 401. O papel insuficiente volta 403.

Essa integração ataca ao mesmo tempo o gerenciamento de informações e a comunicação entre equipes. O gestor deixa de anotar a matrícula fora do chamado, e o colaborador deixa de pedir suporte por canal sem registro. Os dois veem o status devolvido pela API, não uma confirmação local.

## 6.2 Portal do gestor

O gestor autentica com a conta de configuração. O token traz o papel Gestor e não traz `alunoId`. Por isso o formulário pede o identificador do colaborador tanto na matrícula quanto no chamado. A autorização deixa esse papel operar qualquer aluno. O portal não lista turmas, não edita cadastro e não consulta histórico. É o cliente mínimo que exercita o contrato. A ausência de listagem é limite do incremento, não uma tela esquecida no texto: não existe endpoint de leitura no repositório, e o portal não finge que existe.

## 6.3 Cliente móvel do colaborador

O colaborador autentica com o e-mail presente em `Alunos` e com a senha de desenvolvimento do papel. Sem esse cadastro, o login responde 401, ainda que a senha comum esteja correta. O token devolve o `alunoId`, e os formulários seguintes não pedem outro aluno. A matrícula pede o curso. O chamado pede título, descrição e prioridade, com o valor médio já selecionado. Se o identificador enviado não for o do claim, a API recusa. O cliente, portanto, não é apenas uma página estreita: ele está sujeito à mesma autorização do portal, com um poder menor.

Não há pacote nas lojas, nem projeto nativo. O cliente móvel entregue é a página responsiva servida pela API. Empacotar essa interface é evolução. Chamá-la de aplicativo publicado seria inexato. Chamá-la de proposta também seria inexato, porque o arquivo existe e chama os endpoints reais.

## 6.4 O que permanece proposta

O módulo desktop, detalhado no capítulo de engenharia, não tem projeto neste repositório. A diferença entre os três clientes é exatamente essa. Web e móvel estão no código que a imagem publica. O desktop está descrito como consumidor futuro do mesmo token e dos mesmos verbos, inclusive de um lote que a API ainda não expõe. A arquitetura de integração já suporta esse terceiro cliente, desde que ele não abra conexão própria com o banco e não duplique a regra que o índice único protege.

## 6.5 Limites comuns aos dois clientes

Os dois clientes compartilham o mesmo limite de produto. Não há tela de cadastro de aluno, não há listagem e não há edição. Quem demonstra o colaborador precisa de uma linha já gravada em `Alunos`, criada pela migration, pelo script ou por um insert administrativo. O gestor de desenvolvimento entra sem essa linha, porque a conta dele não é um aluno. Essa assimetria está no login e aparece na interface: um formulário pede o aluno, o outro o recebe do token.

O tratamento de erro também é comum. A página mostra o status e o corpo, sem traduzir cada falha para um texto próprio. Assim, a mensagem de domínio continua vindo da API, e uma mudança de validação não exige republicar regra no JavaScript. O token fica só na memória da página. Recarregar pede login de novo. Não há armazenamento durável no navegador. É pouco confortável e é adequado ao cliente mínimo, porque evita deixar credencial em arquivo estático.

A escalabilidade desses clientes é a da API que os serve. Não há estado de sessão no servidor. Qualquer réplica futura, quando o manifesto for aplicado, atende a página e o verbo, desde que enxergue o mesmo banco e a mesma chave. Até lá, a integração comprovável é local: a mesma origem, o mesmo token e as mesmas rotas que os manifestos e os testes de regra já pressupõem.

O percurso de uma matrícula no cliente móvel resume a integração. O colaborador informa o e-mail cadastrado e a senha do papel. A API procura o aluno e devolve token e identificador. A página pede somente o curso e envia o identificador recebido, com o bearer. O caso de uso valida aluno, curso e duplicidade. O banco grava a linha sob o índice único. A página mostra o identificador ou a mensagem de domínio. O portal do gestor repete os mesmos verbos, com a diferença de que o operador escolhe o aluno. Nenhum dos dois clientes abre conexão com o PostgreSQL. Esse é o sentido concreto de aplicações web e móvel integradas: duas interfaces, um contrato e uma base. O desktop proposto entraria como terceira interface nesse mesmo percurso, e só então poderia ser descrito como desenvolvido.

O mesmo percurso, visto pelo gestor, fecha a comunicação entre quem capacita e quem pede suporte. O gestor entra com a conta de configuração, escolhe o aluno e o curso e recebe o identificador da matrícula. Se o par já existe, a mensagem de domínio aparece na página, vinda da API, sem uma segunda regra escrita no navegador. Em seguida o mesmo aluno pode ser indicado num chamado, com prioridade baixa, média ou alta. O colaborador, no outro cliente, só consegue abrir chamado para si. Os dois registros ficam no mesmo banco, ligados pelo mesmo identificador de aluno. A integração pedida no enunciado está nesse encontro: não são dois sistemas que depois serão conectados, são duas telas que já nascem no contrato único. O que ainda não nasce dali é a fila visual de chamados e a matrícula em lote na tela. Ambas dependem de leitura e de um endpoint que o repositório não tem, e por isso permanecem na proposta do desktop, não numa captura inexistente destes clientes.

## 6.6 O que a página não decide

A página não escolhe o status da matrícula. Quem grava o valor ativo é o caso de uso, e o lote no banco grava o mesmo valor quando é chamado à parte. A página não escolhe a data. O carimbo nasce na entidade ou, no lote, em `NOW()`. A página não decide se o colaborador pode agir por outro aluno: isso é claim e papel, conferidos antes do caso de uso. Se a conferência falha, o formulário só exibe o 403. Não há ramo de JavaScript que grave mesmo assim.

Também não há segredo de produção dentro do HTML. A senha de desenvolvimento que aparece sugerida no portal do gestor é a mesma conta local já usada para o ensaio, e não uma credencial de cluster. O colaborador não recebe senha gravada na página além do valor inicial de desenvolvimento, que o operador substitui. O token não vai para `localStorage`. Fechar ou recarregar encerra a sessão naquele navegador. Essa escolha reduz conforto e reduz o tempo em que um terminal compartilhado permaneceria autenticado, o que combina com o uso corporativo pretendido, em que a máquina do colaborador não é um cofre.

Por fim, a página não substitui o pipeline. Ela entra na imagem porque está em `wwwroot` e o publish a copia. Um cluster futuro, ao puxar essa imagem, entrega as duas interfaces junto com a API, desde que o Secret e o banco existam. Enquanto o push não ocorre, a prova da integração é local: subir a API, abrir `/gestor/` e `/colaborador/` e percorrer login, matrícula e chamado. É essa prova que autoriza chamar os dois clientes de desenvolvidos. O desktop, sem projeto e sem esse percurso, continua de fora.

# 7. Conclusão

O objetivo geral era entregar uma solução tecnológica corporativa integrada para a Alunos Tech, com nuvem e DevOps, gestão ágil, programação .NET e programação de banco. No escopo deste PIM, esse objetivo foi atingido na Plataforma de Gestão de Cursos e Suporte Corporativo: a mesma API matricula e abre chamado, o PostgreSQL guarda o vínculo e o repositório registra como o ambiente é reproduzido e como ele ainda não sobe em cluster.

Os objetivos específicos foram confrontados com o que está no código. As aplicações web e móvel foram desenvolvidas e chamam os endpoints reais. O banco foi estruturado com chaves, índice único, procedimento de lote que grava a linha inteira, função, gatilho, transação do chamador e papel sem `DELETE`. As práticas de nuvem e DevOps aplicadas são a imagem Docker, o Compose do PostgreSQL, o pipeline que constrói a imagem sem publicá-la e os manifestos Kubernetes com as sondas já implementadas. A gestão do projeto usou backlog, quatro Sprints, Kanban e um incremento de adequação. A segurança aplicada está no JWT, na autorização por papel e no privilégio mínimo do banco. A arquitetura em camadas foi proposta e construída. O módulo desktop foi somente proposto.

Permanece como evolução o que não está em execução: publicar a imagem em registro, criar o Secret num cluster, aplicar o Deployment, medir o HPA, acrescentar métricas e alerta, empacotar o cliente móvel e construir o desktop. A conclusão não converte esse roteiro em entrega. O que se sustenta é o software verificável já presente no repositório e a separação explícita entre ele e o próximo passo.

# Referências

BECK, Kent *et al.* **Manifesto para desenvolvimento ágil de software**. 2001. Disponível em: https://agilemanifesto.org/iso/ptbr/manifesto.html. Acesso em: 5 nov. 2026.

DOCKER. **Docker Compose**. 2026a. Disponível em: https://docs.docker.com/compose/. Acesso em: 5 nov. 2026.

DOCKER. **Dockerfile reference**. 2026b. Disponível em: https://docs.docker.com/reference/dockerfile/. Acesso em: 5 nov. 2026.

KUBERNETES. **Documentação do Kubernetes**. 2026. Disponível em: https://kubernetes.io/docs/home/. Acesso em: 5 nov. 2026.

MARTIN, Robert C. **Arquitetura limpa**: o guia do artesão para estrutura e design de software. Rio de Janeiro: Alta Books, 2019. ISBN 978-85-508-0460-6.

MICROSOFT. **Configurar a autenticação do portador JWT no ASP.NET Core**. Redmond, 2023a. Disponível em: https://learn.microsoft.com/pt-br/aspnet/core/security/authentication/configure-jwt-bearer-authentication. Acesso em: 5 nov. 2026.

MICROSOFT. **Documentação do C#**. Redmond, 2023b. Disponível em: https://learn.microsoft.com/pt-br/dotnet/csharp/. Acesso em: 5 nov. 2026.

MICROSOFT. **Entity Framework Core**. Redmond, 2023c. Disponível em: https://learn.microsoft.com/pt-br/ef/core/. Acesso em: 5 nov. 2026.

MICROSOFT. **Novidades no .NET 8**. Redmond, 2023d. Disponível em: https://learn.microsoft.com/pt-br/dotnet/core/whats-new/dotnet-8/overview. Acesso em: 5 nov. 2026.

MICROSOFT. **Verificações de integridade no ASP.NET Core**. Redmond, 2023e. Disponível em: https://learn.microsoft.com/pt-br/aspnet/core/host-and-deploy/health-checks. Acesso em: 5 nov. 2026.

NPGSQL. **Npgsql Entity Framework Core provider**. 2026. Disponível em: https://www.npgsql.org/efcore/. Acesso em: 5 nov. 2026.

POSTGRESQL GLOBAL DEVELOPMENT GROUP. **PostgreSQL 16.15 documentation**. 2026. Disponível em: https://www.postgresql.org/docs/16/index.html. Acesso em: 5 nov. 2026.

SCHWABER, Ken; SUTHERLAND, Jeff. **O guia do Scrum**: o guia definitivo para o Scrum: as regras do jogo. 2020. Disponível em: https://scrumguides.org/docs/scrumguide/v2020/2020-Scrum-Guide-PortugueseBR-3.0.pdf. Acesso em: 5 nov. 2026.
