# Minha parte no RECRIAR — banco de dados

## O que ficou na minha parte

Fiquei responsável pela estrutura do banco e pelo código que salva e consulta os cadastros no MySQL. A ideia foi fazer essa parte funcionar com as classes que o grupo já tinha preparado, sem precisar mudar os campos usados pelo serviço e pelas telas.

Preparei as tabelas, os relacionamentos, os comandos SQL, os diagramas e as classes de acesso ao banco. Depois, juntei essa parte com as telas do Pedro e fiz a ligação no início do programa. Também ficou pronta a estrutura de oficinas e presenças. Essa parte das oficinas ainda não tem telas nem serviços próprios.

## Como organizei o banco

Comecei olhando as classes do projeto e o guia que o grupo mandou. Isso foi importante pra usar os mesmos nomes, tamanhos de campos e regras do código, em vez de montar um banco que depois não encaixasse no programa.

O banco principal ficou com 11 tabelas:

| Tabela | O que guarda |
|---|---|
| `Children` | Dados da criança e da ficha de cadastro. |
| `Guardians` | O responsável informado no cadastro. |
| `EmergencyContacts` | Nome e telefone do contato de emergência. |
| `FamilyMembers` | Os familiares de cada criança. |
| `RelationshipTypes` | As opções de parentesco. |
| `ActivityLogs` | O histórico das operações do sistema. |
| `Programs` | Os programas de atendimento. |
| `Activities` | As atividades e oficinas de cada programa. |
| `ActivitySessions` | Os encontros, com horário e local. |
| `SessionEnrollments` | As inscrições das crianças nos encontros. |
| `AttendanceRecords` | A presença ou falta em cada inscrição. |

As cinco tabelas de oficinas foram incluídas pra completar a quantidade pedida pelo grupo com uma estrutura que pudesse ser aproveitada pela ONG. Assim dá pra relacionar uma criança com os encontros em que ela participa, sem misturar esses dados com a ficha do cadastro.

## Por que separei os dados em tabelas

Cada tabela ficou responsável por um assunto. Os familiares, por exemplo, são registros separados ligados à criança. Isso permite adicionar ou remover um familiar sem precisar criar colunas como familiar1, familiar2 e familiar3.

Os relacionamentos usam chaves primárias e estrangeiras. A chave primária identifica o registro, como o `Id` da criança. A chave estrangeira aponta para outro registro, como o `ChildId` de um familiar.

O responsável e o contato de emergência têm um vínculo único com a criança. Isso impede cadastrar dois responsáveis ou dois contatos na mesma ficha. Essa foi a estrutura definida nas classes do grupo: os dados do responsável pertencem à ficha, sem um cadastro único de pessoas compartilhado entre todas as crianças.

A organização segue a terceira forma normal considerando essas regras. Por exemplo, o nome do parentesco fica na tabela de parentescos, em vez de ser repetido em cada familiar. Se o grupo mudar a forma de identificar pessoas ou transformar escola e cidade em tabelas próprias, a modelagem também precisa ser revista.

## Algumas regras que mantive

CPF, RG e telefone ficaram como texto, porque são identificadores e podem começar com zero. Não coloquei CPF como único, já que o código original permite repetir. As idades continuam aceitando de 0 a 120 anos, conforme a validação que já estava no projeto.

Os tamanhos das colunas seguem os limites usados pelo serviço. Os campos de endereço, escola, centro de saúde, ficha e alergia podem ficar vazios. O campo `OtherRelationship` também pode receber NULL quando a opção escolhida não for Outro.

A lista inicial tem nove parentescos. Mantive os códigos fixos e deixei Outro com o Id 9, porque o serviço usa esse número pra saber quando precisa pedir uma descrição. O limite de dez familiares também continua sendo conferido pelo serviço.

Ao excluir uma criança, o banco remove os dados vinculados a ela, como responsável, contato, familiares, inscrições e presenças. Isso é feito com `ON DELETE CASCADE`. Já um parentesco que está em uso não pode ser apagado, por causa da regra `RESTRICT`.

Nas oficinas, uma criança não pode ter duas inscrições no mesmo encontro. Também só pode existir um registro de presença por inscrição, e o horário de término do encontro precisa ser depois do início.

## Como fiz a comunicação com o C#

A versão atual usa ADO.NET com SQL direto, seguindo o tipo de exemplo mostrado na foto da aula. Antes havia uma versão com Entity Framework, mas adaptei o acesso para deixar os comandos SQL mais visíveis e próximos do conteúdo apresentado pelo professor.

Usei `MySqlConnection` pra abrir a conexão e `MySqlCommand` pra executar os comandos. Os três métodos principais são:

- `ExecuteReader`: lê os resultados de uma consulta, como a lista de crianças.
- `ExecuteNonQuery`: executa inclusões, alterações e exclusões e informa quantas linhas foram afetadas.
- `ExecuteScalar`: pega um único valor, como o Id gerado depois de cadastrar uma criança.

Os valores são enviados por parâmetros, como `@id` e `@name`. Fiz assim pra não juntar diretamente o texto digitado pela pessoa com o comando SQL, o que ajuda a evitar SQL injection.

As consultas e gravações ficaram nas classes `ChildRepository` e `RelationshipTypeRepository`. Os nomes e as interfaces foram mantidos pra facilitar a ligação com o serviço que o grupo já tinha feito.

## Como liguei o banco às telas do Pedro

As telas chegaram pelo PR #9, que já foi colocado na branch sandbox. Antes dessa integração, elas tinham listagem, pesquisa, cadastro, edição, exclusão e exportação, mas ainda faltava fornecer os componentes reais do MySQL ao serviço.

Juntei as duas partes em uma branch de integração e alterei o `Program.cs`. O método `CreateService`, que antes devolvia null, agora cria os repositórios do banco e o registro de atividades e os passa para o `ChildService`. A tela principal recebe esse serviço para realizar as operações.

Também coloquei uma conferência na abertura: o programa tenta conectar ao banco de gravação e consulta os parentescos no banco de leitura. Ele verifica se Outro está cadastrado com o Id 9. Se essa preparação falhar, mostra uma mensagem orientando a conferir o MySQL, os dois bancos e as variáveis de conexão, e registra o erro no arquivo de log.

Mantive o desenho e as funções das telas que o Pedro fez. Não foi preciso mudar as classes de cadastro nem as validações do grupo. As conexões são abertas e fechadas dentro de cada operação, então não ficam abertas enquanto a pessoa preenche uma ficha.

## Como funciona o cadastro e a edição

No cadastro, o código inclui a criança e depois os dados de responsável, contato e familiares. Tudo isso fica dentro de uma transação. Se alguma parte falhar antes da confirmação, as alterações são desfeitas. Assim não fica uma ficha salva pela metade.

Na edição, os registros existentes são atualizados. Os familiares novos são incluídos e os que saíram da ficha são removidos. Os familiares que continuam mantêm seus IDs. Essa operação também usa uma transação.

O histórico registra operações como cadastro, edição, exclusão e exportação. Ele usa outra conexão, porque uma falha ao registrar a atividade não deve desfazer um cadastro que já foi salvo. O histórico permanece depois da exclusão da criança e os horários são guardados em UTC.

## Como ficaram produção e leitura

Preparei dois bancos no mesmo servidor MySQL:

- `recriar_prod`: guarda os dados nas 11 tabelas e recebe as gravações.
- `recriar_leitura`: tem 11 views para consultar os dados atuais do principal.

Uma view é uma consulta salva. Ela mostra os dados do banco principal, sem guardar outra cópia. Por isso, depois de alterar um cadastro no principal, a próxima consulta na leitura já mostra a mudança.

As views foram criadas sem permitir gravação. Também preparei papéis de acesso separados: um para consultar e alterar o principal e outro que permite somente consultas na leitura. As senhas ficam fora do código e as duas conexões são configuradas por variáveis de ambiente.

**Ponto pra confirmar com o grupo:** a orientação recebida pediu produção e leitura, mas não explicou se precisava de replicação. A solução atual usa dois bancos lógicos no mesmo servidor, com views. Ela não é uma cópia física nem uma réplica independente. Se o professor pediu isso, essa parte precisa ser ajustada.

## Diagramas e arquivos da entrega

Preparei o MER para mostrar as entidades principais e seus relacionamentos. Também gerei o EER a partir do banco no MySQL Workbench, com as tabelas, os campos e as chaves. O arquivo `.mwb` permite abrir e editar o modelo no Workbench.

O `database.sql` reúne a instalação dos dois bancos. Os dados fictícios ficam separados em `sample-data.sql` e as consultas de exemplo em `queries.sql`. Separei os exemplos da instalação pra não colocar cadastros fictícios automaticamente no banco principal.

## O que foi testado e o que ainda falta

O projeto compilou sem erros nem avisos e passou por 42 verificações no MySQL. Foram conferidos o cadastro, a leitura da ficha completa, a edição dos familiares, a pesquisa por nome, as exclusões e as regras dos relacionamentos.

Também foram testadas falhas no meio de um cadastro e de uma edição, pra conferir se a transação desfazia o conjunto. Na parte de leitura, foi verificado que a conta não consegue alterar os dados e que as mudanças do principal aparecem nas consultas.

Os testes usaram dados fictícios em um ambiente local separado. Também foi gerado o HTML real a partir do MySQL, conferindo os dados da criança, do responsável e dos familiares. Uma nova instância do acesso ao banco conseguiu buscar os dados e os IDs já salvos.

Na interface, foi conferida a abertura da tela principal conectada ao MySQL, a pesquisa por parte do nome e a abertura da ficha para edição. Também foi verificada a mensagem ao tentar iniciar com uma conexão indisponível.

Ainda falta percorrer manualmente o fluxo inteiro pelos botões das telas, incluindo salvar um cadastro novo, alterar a composição familiar, fechar e reabrir o aplicativo, exportar pelo botão e excluir pela interface. Essas operações já têm verificações pelo serviço, mas isso não substitui o teste completo pela tela. O relatório de integração indica exatamente o que foi conferido.

A ligação das telas com o MySQL no `Program.cs` está implementada localmente. As tabelas de oficinas continuam prontas, mas as funções e as telas para usar as oficinas ainda precisam ser feitas. A implantação na ONG e a confirmação do modelo de leitura com o professor também continuam pendentes.
