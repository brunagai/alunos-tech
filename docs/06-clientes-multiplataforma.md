# 6. Clientes web e móvel

## 6.1 Integração pela mesma API

Desenvolver aplicações web e móvel, no enunciado, não se cumpre com um desenho de tela. O portal e a página de navegador estão em `wwwroot` e são servidos pela API. O aplicativo é o projeto MAUI, fora dessa pasta, e chama o mesmo host. Não há política de CORS no navegador porque ele fala com o host que entregou a página. O portal do gestor responde em `/gestor/`. A página do colaborador responde em `/colaborador/`, com viewport móvel. Os três clientes usam `POST /api/auth/login`, `POST /api/matriculas` e `POST /api/chamados`. O corpo da matrícula leva aluno e curso. O corpo do chamado leva aluno, título, descrição e prioridade. A resposta de sucesso devolve o identificador. A falha de domínio volta 400. A credencial recusada volta 401. O papel insuficiente volta 403.

Essa integração ataca ao mesmo tempo o gerenciamento de informações e a comunicação entre equipes. O gestor deixa de anotar a matrícula fora do chamado, e o colaborador deixa de pedir suporte por canal sem registro. Os dois veem o status devolvido pela API, não uma confirmação local.

## 6.2 Portal do gestor

O gestor autentica com a conta de configuração. O token traz o papel Gestor e não traz `alunoId`. Por isso o formulário pede o identificador do colaborador tanto na matrícula quanto no chamado. A autorização deixa esse papel operar qualquer aluno. O portal não lista turmas, não edita cadastro e não consulta histórico. É o cliente mínimo que exercita o contrato. A ausência de listagem é limite do incremento, não uma tela esquecida no texto: não existe endpoint de leitura no repositório, e o portal não finge que existe.

## 6.3 Cliente móvel do colaborador

O aplicativo do colaborador é o projeto `src/Mobile/AlunosTech.Mobile`, em .NET MAUI, com tela própria de login, matrícula e chamado. O login usa o e-mail `colaborador@alunostech.local`; o token traz o `alunoId`, e a tela envia a matrícula e o chamado com esse identificador. No emulador, com `adb reverse` da porta 18080, o log registrou `MATRICULA 201`. A página `/colaborador/` é um cliente web de navegador, não este aplicativo. Não há pacote na App Store nem na Play Store. O alvo `net8.0-windows10.0.19041.0` compilou, mas o processo morreu com `REGDB_E_CLASSNOTREG` antes de qualquer HTTP.

O alvo Android compilou depois de instalada a API 34 e de o build sair de um caminho sem o caractere especial do diretório. O APK de depuração rodou no emulador `Medium_Phone_API_37.0` e usou o curso semeado no PostgreSQL do cluster. A autorização é a mesma do portal, com um poder menor: o aplicativo não envia outro aluno além do claim.

## 6.4 O que permanece proposta

O módulo desktop, detalhado no capítulo de engenharia, não tem projeto neste repositório. O portal web e a página de navegador estão na imagem. O aplicativo MAUI é outro projeto e foi executado no emulador. O desktop segue apenas descrito, como consumidor futuro do mesmo token, sem WinForms e sem WPF.

## 6.5 Limites comuns aos dois clientes

Os dois clientes compartilham o mesmo limite de produto. Não há tela de cadastro de aluno, não há listagem e não há edição. Quem demonstra o colaborador precisa de uma linha já gravada em `Alunos`, criada pela migration, pelo script ou por um insert administrativo. O gestor de desenvolvimento entra sem essa linha, porque a conta dele não é um aluno. Essa assimetria está no login e aparece na interface: um formulário pede o aluno, o outro o recebe do token.

O tratamento de erro também é comum. A página mostra o status e o corpo, sem traduzir cada falha para um texto próprio. Assim, a mensagem de domínio continua vindo da API, e uma mudança de validação não exige republicar regra no JavaScript. O token fica só na memória da página. Recarregar pede login de novo. Não há armazenamento durável no navegador. É pouco confortável e é adequado ao cliente mínimo, porque evita deixar credencial em arquivo estático.

A escalabilidade desses clientes é a da API que os serve. Não há estado de sessão no servidor. As réplicas do Deployment no kind atendem a página e o verbo, desde que enxerguem o mesmo banco interno e a mesma chave. A chamada do emulador usou esse Service.

O percurso da matrícula no aplicativo MAUI resume a integração móvel. O colaborador entra com o e-mail cadastrado e a senha do papel. A API devolve o token e o identificador. O aplicativo envia `POST /api/matriculas` com esse token para `127.0.0.1:18080`, porta alcançada no emulador por `adb reverse`. O log do processo registrou HTTP 201. A página `/colaborador/` é um cliente web adicional, de navegador, e não foi essa a matrícula executada no emulador. O portal do gestor continua o cliente web e escolhe o aluno. Nenhum desses clientes abre conexão com o PostgreSQL. O desktop segue só proposta.

O mesmo percurso, visto pelo gestor, fecha a comunicação entre quem capacita e quem pede suporte. O gestor entra com a conta de configuração, escolhe o aluno e o curso e recebe o identificador da matrícula. Se o par já existe, a mensagem de domínio aparece na página, vinda da API, sem uma segunda regra escrita no navegador. Em seguida o mesmo aluno pode ser indicado num chamado, com prioridade baixa, média ou alta. O colaborador, no aplicativo, só consegue abrir chamado para si. Os dois registros ficam no mesmo banco, ligados pelo mesmo identificador de aluno. A integração pedida no enunciado está nesse encontro: não são dois sistemas que depois serão conectados, são o portal, a página de navegador e o aplicativo no mesmo contrato. O que ainda não nasce dali é a fila visual de chamados e a matrícula em lote na tela. Ambas dependem de leitura e de um endpoint que o repositório não tem, e por isso permanecem na proposta do desktop, não numa captura inexistente destes clientes.

## 6.6 O que a página não decide

A página não escolhe o status da matrícula. Quem grava o valor ativo é o caso de uso, e o lote no banco grava o mesmo valor quando é chamado à parte. A página não escolhe a data. O carimbo nasce na entidade ou, no lote, em `NOW()`. A página não decide se o colaborador pode agir por outro aluno: isso é claim e papel, conferidos antes do caso de uso. Se a conferência falha, o formulário só exibe o 403. Não há ramo de JavaScript que grave mesmo assim.

Também não há segredo de produção dentro do HTML. A senha de desenvolvimento que aparece sugerida no portal do gestor é a mesma conta local já usada para o ensaio, e não uma credencial de cluster. O colaborador não recebe senha gravada na página além do valor inicial de desenvolvimento, que o operador substitui. O token não vai para `localStorage`. Fechar ou recarregar encerra a sessão naquele navegador. Essa escolha reduz conforto e reduz o tempo em que um terminal compartilhado permaneceria autenticado, o que combina com o uso corporativo pretendido, em que a máquina do colaborador não é um cofre.

A página entra na imagem porque está em `wwwroot`. A imagem publicada no registro local já contém o portal e a página. O aplicativo não está dentro dessa imagem: ele é o APK que chamou a API do cluster. O desktop, sem projeto e sem esse percurso, continua de fora.

## 6.7 Apresentação na ordem de uso

O percurso executado começa no emulador do aplicativo, não numa tela desenhada. A Figura 3 mostra o login já preenchido com colaborador@alunostech.local.

[[figura:figuras/maui-login.png|5.2]]
[[legenda:Figura 3 — Login do aplicativo MAUI no emulador]]
[[fonte:Fonte: Elaborada pela autora.]]

A matrícula seguinte, no curso 55555555-5555-5555-5555-555555555555, devolveu HTTP 201. A Figura 4 é essa resposta na tela do emulador.

[[figura:figuras/maui-matricula.png|5.2]]
[[legenda:Figura 4 — Matrícula do aplicativo com HTTP 201]]
[[fonte:Fonte: Elaborada pela autora.]]

O chamado aberto na mesma tela, com o título “Chamado pelo aplicativo”, também devolveu HTTP 201.

[[figura:figuras/maui-chamado.png|5.2]]
[[legenda:Figura 5 — Chamado aberto pelo aplicativo MAUI]]
[[fonte:Fonte: Elaborada pela autora.]]

O portal do gestor repete o contrato no navegador, em /gestor/. A Figura 6 é o login da conta gestor@alunostech.local.

[[figura:figuras/portal-login.png|12]]
[[legenda:Figura 6 — Login do portal do gestor]]
[[fonte:Fonte: Elaborada pela autora.]]

A matrícula do curso 44444444-4444-4444-4444-444444444444 devolveu HTTP 201 para o aluno do colaborador.

[[figura:figuras/portal-matricula.png|9]]
[[legenda:Figura 7 — Matrícula do portal com HTTP 201]]
[[fonte:Fonte: Elaborada pela autora.]]

O chamado do mesmo aluno, “Chamado pelo portal”, devolveu HTTP 201.

[[figura:figuras/portal-chamado.png|9]]
[[legenda:Figura 8 — Chamado aberto pelo portal do gestor]]
[[fonte:Fonte: Elaborada pela autora.]]

A matrícula repetida do par que já existia devolveu HTTP 400, com a mensagem de domínio na própria página.

[[figura:figuras/portal-duplicada.png|9]]
[[legenda:Figura 9 — Matrícula duplicada recusada pelo portal]]
[[fonte:Fonte: Elaborada pela autora.]]

A página /colaborador/ é um cliente web de navegador. A Figura 10 não substitui o emulador das Figuras 3 a 5.

[[figura:figuras/navegador-colaborador.png|10]]
[[legenda:Figura 10 — Cliente web do colaborador no navegador]]
[[fonte:Fonte: Elaborada pela autora.]]
