# Minha parte no RECRIAR

## O que fiz

Fiquei com a parte do banco de dados. Preparei as tabelas no MySQL, os relacionamentos e o código que salva e consulta os cadastros. Depois juntei essa parte com as telas do Pedro, que vieram pelo PR #9.

Comecei pelas classes e pelo guia do grupo pra manter os mesmos campos e regras. Assim, a parte do banco consegue trabalhar com o serviço que já existia no projeto.

## Organização das tabelas

O banco principal tem 11 tabelas:

| Tabela | O que guarda |
|---|---|
| Children | Os dados da criança e da ficha. |
| Guardians | O responsável informado no cadastro. |
| EmergencyContacts | O contato de emergência. |
| FamilyMembers | Os familiares da criança. |
| RelationshipTypes | A lista de parentescos. |
| ActivityLogs | O histórico das operações. |
| Programs | Os programas de atendimento. |
| Activities | As atividades e oficinas. |
| ActivitySessions | Os encontros, com horário e local. |
| SessionEnrollments | As inscrições nos encontros. |
| AttendanceRecords | A presença ou falta de cada inscrição. |

Separei os dados por assunto. Os familiares, por exemplo, ficam em registros próprios ligados à criança, em vez de criar várias colunas para familiar 1, familiar 2 e assim por diante.

Cada registro tem um Id, que é a chave primária. A chave estrangeira faz a ligação com outra tabela. O ChildId de um familiar indica a qual criança ele pertence.

As cinco tabelas de oficinas completam as 11 pedidas e permitem organizar a participação das crianças nas atividades. Essa estrutura está pronta no banco, mas ainda não tem telas nem funções próprias no programa.

## Regras que mantive

Usei os mesmos limites de tamanho dos campos que o serviço já valida. CPF, RG e telefone são textos pra não perder o zero à esquerda. O CPF pode se repetir e as idades aceitas vão de 0 a 120 anos, como no código original.

Cada ficha pode ter no máximo um responsável e um contato de emergência. A lista inicial tem nove parentescos, e Outro ficou com o Id 9 porque o serviço usa esse número para pedir a descrição. O limite de dez familiares também continua sendo conferido pelo serviço.

Quando uma criança é excluída, os dados vinculados são removidos junto, como responsável, contato, familiares, inscrições e presenças. Essa é a regra ON DELETE CASCADE. Já um parentesco em uso não pode ser apagado, por causa do RESTRICT. O histórico das operações continua salvo depois de excluir a criança.

Nas oficinas, não pode haver duas inscrições da mesma criança no mesmo encontro nem duas presenças para a mesma inscrição. O encontro também precisa terminar depois do horário em que começou.

## Como o C# acessa o banco

Usei ADO.NET com SQL direto, seguindo o tipo de exemplo mostrado na foto da aula. MySqlConnection abre a conexão e MySqlCommand executa o SQL.

- ExecuteReader lê os registros de uma consulta.
- ExecuteNonQuery executa INSERT, UPDATE e DELETE.
- ExecuteScalar retorna um único valor, como o Id gerado no cadastro.

Os valores são passados por parâmetros, como @id e @name. Isso evita colocar diretamente no SQL o texto digitado pela pessoa e ajuda a proteger as consultas contra SQL injection.

O cadastro e a edição usam transações. Se der erro em uma das partes, as alterações ainda não confirmadas são desfeitas. Isso evita salvar a criança com metade da ficha faltando.

Na edição dos familiares, o código atualiza os que continuam, inclui os novos e remove os que saíram da ficha. Os familiares mantidos continuam com o mesmo Id. As conexões são abertas e fechadas em cada operação.

## Produção e leitura

Preparei o recriar_prod para guardar os dados nas 11 tabelas e receber as gravações. O recriar_leitura tem 11 views que consultam esses dados. Uma view é uma consulta salva, então ela mostra os dados atuais do principal sem guardar outra cópia.

As views não permitem gravação. Também preparei permissões separadas para a conta de gravação e para a conta de consulta. As senhas ficam fora do código, e os dados das duas conexões são configurados por variáveis de ambiente.

Essa solução usa dois bancos no mesmo servidor, com views na leitura. Ainda precisa confirmar com o professor se é esse o modelo pedido ou se ele quer uma réplica independente. A mensagem e a foto recebidas não detalham isso.

## Ligação com as telas

As telas do Pedro já tinham listagem, pesquisa, cadastro, edição, exclusão e exportação, mas ainda faltava ligar o serviço ao MySQL. Fiz essa ligação no Program.cs, no método CreateService. Ele cria os componentes reais do banco e entrega o ChildService à tela principal.

Mantive os formulários, as classes de cadastro e as validações do grupo. Na abertura, o programa confere a conexão e o parentesco Outro com Id 9. Se não conseguir iniciar, mostra uma orientação de configuração e registra o erro em arquivo.

Também preparei o registro de atividades para guardar cadastro, edição, exclusão e exportação. Ele usa outra conexão, porque uma falha no histórico não deve desfazer um cadastro que já foi salvo.

## Diagramas e testes

Preparei o MER com as entidades e os relacionamentos e gerei o EER no MySQL Workbench a partir do banco. A entrega também tem os scripts de instalação, dados fictícios separados e consultas de exemplo.

O projeto compilou sem erros nem avisos e passou por 42 verificações no MySQL. Foram testados cadastro, pesquisa, edição dos familiares, exclusão dos dados vinculados, transações e permissões de leitura. Também foi gerado e conferido o HTML real com os dados do banco.

Na interface, foram conferidas a abertura do programa, a pesquisa, a abertura de uma ficha para edição e a mensagem quando a conexão está indisponível.

Ainda falta fazer uma rodada manual completa pelos botões: salvar um cadastro novo, editar os familiares, fechar e reabrir o programa, exportar e excluir a ficha. Os testes usaram dados fictícios em um ambiente local, não uma instalação na ONG.
