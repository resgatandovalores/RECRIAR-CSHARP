-- Executar como administrador após criar os dois bancos.
-- Os papéis não contêm senhas nem criam contas de login.
CREATE ROLE IF NOT EXISTS 'recriar_gravacao', 'recriar_consulta';
GRANT SELECT, INSERT, UPDATE, DELETE ON recriar_prod.* TO 'recriar_gravacao';
GRANT SELECT ON recriar_leitura.* TO 'recriar_consulta';

-- Depois de criar duas contas LOCAIS no Workbench, por exemplo recriar_app e recriar_reader:
-- GRANT 'recriar_gravacao' TO 'recriar_app'@'localhost';
-- SET DEFAULT ROLE 'recriar_gravacao' TO 'recriar_app'@'localhost';
-- GRANT 'recriar_consulta' TO 'recriar_reader'@'localhost';
-- SET DEFAULT ROLE 'recriar_consulta' TO 'recriar_reader'@'localhost';
-- Use contas novas, sem permissões globais ou outros papéis que permitam gravação.
