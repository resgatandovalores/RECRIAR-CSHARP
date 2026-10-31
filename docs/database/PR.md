# feat: implementa persistência MySQL do RECRIAR

Destino: sandbox

Implementa AppDbContext, configurações, migration inicial, seed dos nove parentescos e repositórios síncronos dos contratos existentes. O logger de atividades usa um contexto independente por chamada. As classes de Domain, Application e as telas foram preservadas.

O banco possui 11 tabelas de negócio. As cinco adicionais representam programas, atividades, encontros, inscrições por encontro e presenças. A extensão foi modelada e testada; não adiciona telas ou serviços de oficinas.

Inclui SQL completo com dados fictícios e consultas, modelo editável do Workbench, MER e guia de integração. A configuração usa variável de ambiente e a fábrica de desenvolvimento restringe a conexão ao banco recriar_dev em loopback.

Validação: build da solution sem avisos nem erros; 26 verificações aprovadas em MySQL 8.0.46; execução do SQL completo em schema isolado. O formulário do Pedro ainda precisa receber o ChildService no Program.cs. Os testes usam um exportador substituto e não validam a geração HTML real.
