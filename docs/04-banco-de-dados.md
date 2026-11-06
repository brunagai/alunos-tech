# 5. Persistência e programação em banco de dados

## 5.1 Modelo seguro e consultas

O gerenciamento de informações da Alunos Tech cabe em quatro tabelas. `Alunos` guarda identificador, nome, e-mail, cargo opcional, situação e carimbos. `Cursos` guarda título, descrição opcional, carga horária e situação. `Matriculas` guarda aluno, curso, status, data da matrícula e carimbos. `ChamadosSuporte` guarda aluno, título, descrição, status, prioridade e carimbos. As chaves primárias são `uuid`. As estrangeiras de matrícula e de chamado usam `ON DELETE RESTRICT`: o banco recusa apagar aluno ou curso ainda referenciado (POSTGRESQL GLOBAL DEVELOPMENT GROUP, [s.d.]a). O mapeamento é Fluent API pelo Npgsql, nas migrations `InitialCreate` e `AddEntitiesAndMappings` (MICROSOFT, [s.d.]b; NPGSQL, [s.d.]). Não há valor padrão de banco para o identificador da matrícula, para o status nem para as datas. Quem insere precisa preencher a linha.

A otimização está nos índices, não numa reescrita especulativa de consulta. Há índice em `Matriculas.AlunoId`, em `Matriculas.CursoId` e em `ChamadosSuporte.AlunoId`, para o filtro e a junção por um dos lados. O índice único `UX_Matriculas_AlunoId_CursoId` é ao mesmo tempo regra de negócio e controle de concorrência. Duas transações podem não ver a matrícula uma da outra e tentar gravar o mesmo par; a segunda falha no único, ou é ignorada quando o comando usa `ON CONFLICT`. A Figura 2 deixa esse par único visível entre aluno, curso, matrícula e chamado.

[[figura:figuras/modelo.png|14]]
[[legenda:Figura 2 — Aluno, curso, matrícula e chamado, com o par único]]
[[fonte:Fonte: Elaborada pela autora.]]

A segurança do banco não se esgota nesse índice. O script cria o papel `alunos_tech_app`, com uso do schema, `SELECT`, `INSERT` e `UPDATE` nas quatro tabelas, sem `DELETE`, e com execução de `sp_MatricularAlunoEmLote` (Apêndice B – Tabelas e o procedimento). A API local conecta com esse papel. Um ensaio de exclusão foi negado pelo PostgreSQL e a transação foi desfeita. O papel `postgres` continua reservado à administração do schema e às migrations. Senha de desenvolvimento do papel de aplicação está no script, como placeholder, e deve ser trocada fora do repositório antes de qualquer ambiente compartilhado.

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
