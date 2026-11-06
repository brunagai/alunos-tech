# 1. Introdução e contextualização

## 1.1 Apresentação da empresa

A Alunos Tech é uma organização fictícia do segmento de serviços corporativos digitais. Sua atuação consiste em apoiar empresas na capacitação de colaboradores e no atendimento de demandas técnicas internas, por meio de soluções de software sob medida. O público atendido é formado por gestores de equipes, analistas de desenvolvimento e demais profissionais que participam de trilhas de formação continuada no ambiente de trabalho.

No mercado corporativo digital, a empresa posiciona-se como provedora de uma plataforma única para dois fluxos que, com frequência, permanecem separados: a gestão de cursos e matrículas e o registro de chamados de suporte. A proposta não se restringe à disponibilização de conteúdo. Ela organiza o vínculo entre o colaborador, o curso e o chamado técnico, de modo que a área gestora acompanhe a capacitação e o atendimento a partir da mesma base de informações.

Para sustentar essa operação, a Alunos Tech adota um sistema denominado Plataforma de Gestão de Cursos e Suporte Corporativo. O sistema é desenvolvido em .NET 8, com persistência em PostgreSQL 16 e separação de responsabilidades segundo a Clean Architecture. Essa escolha técnica reflete a necessidade de um produto evolutivo, testável e passível de manutenção por equipes de Análise e Desenvolvimento de Sistemas.

## 1.2 Definição do problema de negócios

Organizações que capacitam colaboradores e, ao mesmo tempo, atendem incidentes técnicos costumam distribuir essas atividades em planilhas, caixas de correio e ferramentas sem integração. O registro da matrícula em um curso não conversa com o histórico de chamados do mesmo colaborador. Na Alunos Tech, esse desencontro aparece em cinco dificuldades do enunciado, tratadas de forma concreta pela plataforma.

A integração de sistemas deixa de depender de exportação manual porque o portal do gestor e o cliente móvel do colaborador chamam a mesma API. O gerenciamento de informações concentra aluno, curso, matrícula e chamado em PostgreSQL, com chave estrangeira e índice único. A segurança dos dados combina token JWT, papéis de gestor e de colaborador e um papel de banco sem permissão de exclusão. A comunicação entre equipes passa pelo backlog, pelas Sprints e pelo chamado formal, com título, prioridade e status, em lugar de mensagem solta. A escalabilidade aparece no cluster kind local: duas réplicas iniciais, sondas reais e HPA que, ao ver CPU acima de 70% do pedido, chegou a seis pods.

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

A persistência em PostgreSQL 16 acrescenta mecanismos que o modelo orientado a objetos, sozinho, não oferece no banco: chaves estrangeiras, índice único composto em matrículas, procedimento de matrícula em lote e gatilho de atualização da data de alteração do chamado. O ambiente em contêiner e a entrega executada nesta máquina publicam a imagem no registro local e atualizam o Deployment do cluster kind. O job hospedado no GitHub não faz essa publicação, e não houve registro público nem provedor de nuvem.

Do ponto de vista acadêmico, o trabalho integra conteúdos próprios de Análise e Desenvolvimento de Sistemas: levantamento de requisitos, modelagem de domínio, projeto de banco de dados, programação orientada a objetos, arquitetura de software, desenvolvimento de API, testes automatizados e práticas de DevOps. A empresa fictícia delimita um contexto verossímil, sem depender de dados reais de terceiros, e ainda assim impõe restrições de integridade, rastreabilidade e manutenção comparáveis às de um sistema corporativo.

A relevância do projeto está, portanto, na resolução de um problema de gestão — centralizar capacitação e suporte — por meio de um software estruturado, verificável e documentado. O resultado esperado não é apenas a implementação das funcionalidades de matrícula e de chamado, mas a demonstração de que decisões de arquitetura, de dados e de qualidade podem ser justificadas e sustentadas ao longo do ciclo de desenvolvimento.

## 1.5 Metodologia utilizada

O trabalho adota pesquisa aplicada (GIL, 2008), de natureza tecnológica. O problema parte de uma necessidade concreta da Alunos Tech — centralizar capacitação e suporte — e o resultado é um artefato de software verificável, acompanhado da justificativa das decisões de projeto. A investigação produz conhecimento na medida em que registra como regras de matrícula, de chamado e de persistência foram traduzidas em uma solution executável.

O desenvolvimento apoia-se na engenharia de requisitos. O domínio foi delimitado em colaborador, curso, matrícula e chamado de suporte. De cada elemento foram extraídas restrições observáveis: identificadores obrigatórios, carga horária positiva, prioridade pertencente a um conjunto finito, vínculo obrigatório com o colaborador e unicidade do par colaborador-curso. Essas restrições tornaram-se critérios de aceite das histórias de usuário e, em seguida, validação no domínio, contrato dos casos de uso e restrição no PostgreSQL. O requisito deixa de ser uma descrição solta e passa a ser uma condição que o incremento precisa satisfazer.

A condução do projeto segue práticas ágeis. O Scrum organiza o trabalho em ciclos com objetivo, incremento e revisão (SCHWABER; SUTHERLAND, 2020). O Product Backlog ordena épicos e histórias da Alunos Tech. Quatro Sprints separam infraestrutura e domínio, casos de uso e repositórios, banco avançado e API, qualidade e integração contínua. Um quadro Kanban torna visível o estado de cada história dentro da Sprint e limita o volume de trabalho simultâneo. O detalhamento desses artefatos, dos eventos e da Definition of Done ocupa o capítulo de gerenciamento ágil.

A construção técnica fundamenta-se na arquitetura limpa, nos testes automatizados e na modelagem relacional. A separação entre domínio, aplicação, infraestrutura e interface segue a regra de dependência descrita por Martin (2019): a regra de negócio não referencia o banco nem o HTTP. A verificação da matrícula usa testes de unidade com substitutos dos repositórios, de modo que a regra seja exercida antes da persistência. O estado durável é um modelo relacional em PostgreSQL 16, com chave primária, chave estrangeira, índice e programação no próprio banco, conforme a documentação do SGBD (POSTGRESQL GLOBAL DEVELOPMENT GROUP, [s.d.]c, [s.d.]b). A plataforma .NET 8, o Entity Framework Core e o comportamento da API apoiam-se nas documentações oficiais da Microsoft ([s.d.]b, [s.d.]d). O capítulo de engenharia e o de persistência desenvolvem cada uma dessas escolhas. A metodologia, neste ponto, apenas fixa o caminho: requisito explícito, entrega incremental e fundamento técnico citado.
