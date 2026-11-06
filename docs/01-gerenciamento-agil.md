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

A Sprint 4 teve por objetivo verificar e construir a imagem. Entregou quatro testes aprovados e o pipeline que compila e gera a imagem sem publicá-la. A revisão do manual, feita depois, reabriu o backlog: o procedimento não preenchia as colunas obrigatórias, o pipeline apontava para um caminho antigo, e JWT, sondas, web e móvel existiam só no texto.

O incremento de adequação entregou o login, a autorização, as rotas de saúde, o portal, a página do navegador, o aplicativo MAUI, o `INSERT` completo, o papel de banco e a subida do cluster kind. O desktop permaneceu fora do pronto. Registro público e provedor de nuvem também.

## 3.4 Acompanhamento e melhoria

O acompanhamento não usa percentual de conclusão. Usa a coluna do quadro e o resultado do comando. História em revisão só sai quando o teste passa, o script aplica ou a página chama o endpoint. A retrospectiva deste PIM fixou três correções de processo. A primeira: o capítulo não pode usar o verbo implementar para arquivo inexistente. A segunda: script SQL precisa ser ensaiado no esquema da migration, não num esquema imaginado com valor padrão. A terceira: pipeline e Dockerfile precisam apontar para o mesmo projeto, senão a imagem que o texto chama de construída não constrói.

Essas correções são o conteúdo ágil aplicado à mudança de requisito. O manual exigiu web, móvel, Kubernetes utilizável por sondas reais e pipeline que gera imagem. O backlog absorveu o acréscimo, o incremento entregou o mínimo honesto e a documentação passou a distinguir execução de desenho. É esse o mecanismo de comunicação que substitui a ferramenta solta: um item, um critério e um artefato no repositório.

## 3.5 Quadro e definição de pronto

O quadro usado tem quatro colunas: a fazer, em andamento, em revisão e pronto. O limite de em andamento evita abrir o cliente móvel antes de existir autorização, ou escrever o manifesto antes de existir a rota de saúde. Uma história só entra em revisão com o caminho do arquivo indicado. Só entra em pronto com a verificação correspondente: compilação, teste, script aplicado ou página que chama o verbo.

A definição de pronto da matrícula ilustra o critério. Não basta o caso de uso devolver um identificador no teste. É preciso o endpoint exigir token, o banco recusar o par duplicado e o procedimento, no lote, preencher as colunas que a migration deixou sem padrão. A definição de pronto do chamado ilustra o outro fluxo: prioridade válida, aluno existente e gatilho capaz de renovar `UpdatedAt` sem o cliente enviar a data. A definição de pronto da nuvem local exige o nó Ready, as duas sondas em Healthy, a imagem no registro local e o objeto do HPA. Registro público e provedor ficam fora dessa coluna.

O planejamento de cada Sprint partiu dessas definições, não de uma velocidade inventada. A revisão olhou o incremento. A retrospectiva devolveu ao quadro o que a evidência não sustentava. Esse ciclo é o uso da metodologia ágil neste PIM: poucas cerimônias, critério observável e mudança de escopo registrada quando o manual exigiu mais do que a proposta inicial continha.

A história do cliente móvel saiu de “a fazer” quando o login do colaborador já devolvia `alunoId`. A página `/colaborador/` entrou em revisão ao chamar os três verbos. O aplicativo MAUI só entrou em pronto depois do HTTP 201 no emulador. A história do cluster entrou em pronto com o nó Ready, o push local e o rollout. Registro público continua fora. O quadro é o que impede chamar de loja o APK do emulador.

O limite do quadro também protege o prazo. História sem evidência não avança para abrir outra frente. Por isso o desktop não ocupou a coluna de em andamento ao mesmo tempo que a correção do procedimento: os dois competiriam pela mesma revisão e o texto tenderia a antecipar a tela. O procedimento voltou ao pronto só depois do `CALL` no esquema real, com a segunda chamada inserindo zero linhas e com o `ROLLBACK` limpando o ensaio. O desktop ficou em a fazer, com a arquitetura escrita e sem projeto. Esse corte é a melhoria contínua aplicada a um PIM: menos itens simultâneos e nenhum item promovido por expectativa.
