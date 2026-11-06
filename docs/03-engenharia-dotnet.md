# 4. Engenharia de software e programação em .NET

## 4.1 Objetos, SOLID e módulos

O domínio representa aluno, curso, matrícula e chamado com propriedades de escrita privada. Nome, e-mail, carga horária e prioridade são validados na construção. A falha de negócio é `DomainException`, não um código HTTP (MICROSOFT, [s.d.]c). A plataforma é .NET 8 (MICROSOFT, [s.d.]d). A dependência aponta para dentro: o domínio não referencia aplicação, infraestrutura nem ASP.NET Core (MARTIN, 2019).

A responsabilidade única aparece na divisão da solution `AlunosTech.sln`. `AlunosTech.Domain` guarda entidade, enumeração e exceção. `AlunosTech.Application` guarda DTO, contrato de repositório e caso de uso. `AlunosTech.Infrastructure` guarda o contexto, o mapeamento Fluent, a migration e o repositório. `AlunosTech.Api` guarda controller, filtro, token, sondas e páginas. `AlunosTech.Tests` referencia a aplicação e substitui o repositório. Aberto para extensão, o caso de uso recebe interface; fechado para alteração casual, a regra de duplicidade não mora no controller. A substituição de princípio fica no teste, que não precisa do PostgreSQL. A segregação evita um repositório genérico exposto ao cliente HTTP. A inversão está no registro de injeção: o controller pede o caso de uso, e o caso de uso pede a interface.

## 4.2 Padrões, fluxo e desempenho

O padrão Repository isola a consulta. `AddAsync` chama `SaveChangesAsync` na hora, então não há classe Unit of Work. Cada inclusão é uma transação do Entity Framework. O DTO atravessa a borda HTTP sem expor a entidade. A injeção registra infraestrutura, os dois casos de uso e o emissor de token como singleton. O filtro `DomainExceptionFilter`, registrado nos controllers, devolve 400 com a mensagem de domínio. Não é middleware. `DbUpdateException`, típica do índice único quando duas requisições ganham a corrida, não é convertida em 400.

O fluxo da matrícula é curto. O controller exige o token e confere o papel. O caso de uso recusa identificador vazio, aluno ausente, curso ausente e par já existente, e só então cria a matrícula ativa. O chamado recusa prioridade fora do conjunto e aluno ausente. A leitura dessas checagens percorre a coleção em memória. Isso é simples e é limitado: a lista cresce com a tabela. A garantia sob concorrência não é essa leitura. É o índice único do par aluno-curso, descrito no capítulo de banco. A manutenção futura pode trocar a leitura por consulta filtrada sem mudar o cliente, porque o contrato do endpoint permanece o identificador devolvido.

Os testes de unidade cobrem matrícula válida, aluno inexistente, curso inexistente e matrícula repetida, com Moq e FluentAssertions. A suíte aprovou os quatro fatos. Não há teste de carga, teste do filtro HTTP nem teste de tela. A manutenção se apoia na compilação separada dos projetos e nesse núcleo de regra protegido. O que não está coberto permanece dívida visível, não qualidade implícita.

## 4.3 Módulo desktop proposto

O eixo de programação aplicada pede propor o módulo desktop e a arquitetura, não afirmar que a tela foi construída. A proposta é um projeto à parte na mesma solution, sem referência à infraestrutura e sem cadeia de conexão. Ele chamaria `POST /api/auth/login`, `POST /api/matriculas` e `POST /api/chamados` com o token do gestor, e uma operação futura de lote apontaria para o procedimento já existente, por um endpoint que este repositório ainda não tem. A fila de chamados e a matrícula em lote ficariam nessa tela. A regra continuaria no domínio e no banco.

Não há projeto WinForms, WPF nem outro binário desktop. O projeto MAUI que existe é o aplicativo do colaborador, descrito no capítulo dos clientes, e não ocupa o lugar desta proposta. A arquitetura proposta do desktop continua alinhada ao que a web e o aplicativo fazem: um cliente, um token e a mesma API.

## 4.4 O que a manutenção precisa preservar

A manutenção do núcleo depende de três invariantes. A primeira é a escrita privada do estado: outro projeto não altera o status da matrícula por atribuição solta. A segunda é a exceção de domínio carregando a regra, e o filtro traduzindo só essa exceção. A terceira é o teste do caso de uso sem banco, para que uma mudança de mensagem ou de ordem de validação quebre a suíte antes de quebrar o portal. Quebrar uma dessas invariantes exige revisão, não um ajuste só na página.

O desempenho aceitável para o escopo é o da gravação pontual. Não se promete tempo de resposta sob centenas de usuários, porque não houve ensaio de carga. O ponto já conhecido de custo é a leitura integral na checagem de duplicidade e no login do colaborador. A correção natural, quando a tabela crescer, é uma consulta por e-mail e uma consulta pelo par aluno-curso, mantendo o índice único como última barreira. Antecipar essa consulta no texto sem tê-la no código repetiria o erro que este trabalho corrige.

O módulo desktop, se for construído depois, entra como mais um adaptador. Ele não recebe permissão de apagar linha, porque o papel da API também não recebe. Ele não valida duplicidade sozinho, porque dois clientes divergiriam. A estrutura modular já deixa esse encaixe: novo projeto, mesma aplicação, nenhuma referência ao contexto do Entity Framework.

A suíte atual protege quatro decisões, não a interface. No sucesso, aluno e curso existem, o par é novo e o caso de uso devolve o identificador da matrícula criada. Na ausência de aluno, a exceção ocorre antes de gravar. Na ausência de curso, o mesmo. Na matrícula repetida, a leitura encontra o par e a gravação não é chamada. Os repositórios são substitutos. O teste não abre PostgreSQL e não pede token: autenticação e índice único ficam fora dessa suíte, de propósito, e por isso não são tratados como cobertos por ela. A manutenção dessa fronteira importa. Um teste que passasse a exigir o banco deixaria de ser a prova rápida da regra e passaria a ser um ensaio de integração, que este repositório ainda não possui.
