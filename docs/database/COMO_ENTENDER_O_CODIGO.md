# Por onde começar a entender o código

O cadastro usa classes, propriedades, métodos, listas, condições e tratamento de erros. Esses são os assuntos básicos que você deve estudar primeiro.

A comunicação com o banco usa Entity Framework Core, que já é um assunto intermediário. Ele foi mantido porque o guia do grupo pede AppDbContext, configurações e migrations. A versão atual usa construtores comuns, métodos escritos por inteiro e condições separadas para facilitar a leitura. A estrutura em camadas vem do projeto do grupo.

## Ordem de leitura

1. Abra `Persistence/Entities/ActivityLog.cs`. É uma classe pequena que representa uma linha do histórico.
2. Abra `Persistence/Repositories/ChildRepository.cs`. Comece pelos métodos Add e Delete.
3. Leia GetAll e SearchByName, no mesmo arquivo. Eles fazem as consultas.
4. Leia `RelationshipTypeRepository.cs`, que consulta a lista de parentescos.
5. Leia `AppDbContext.cs`. Ele reúne as tabelas e suas configurações.
6. Por último, leia as configurações e a criação da conexão.

## Como um cadastro é salvo

A tela chama o serviço. O serviço confere os dados e chama Add. Nesse método:

```csharp
_context.Children.Add(child);
_context.SaveChanges();
```

A primeira linha prepara a inclusão da criança. A segunda grava as alterações no MySQL. O responsável, o contato e os familiares que fazem parte do cadastro também são gravados, conforme os relacionamentos configurados.

Para excluir, o código busca a criança com Find. Se não encontrar, devolve false. Se encontrar, usa Remove e SaveChanges. O banco exclui os registros vinculados de acordo com as regras de cascata.

## Os comandos de consulta

| Comando | Para que serve |
|---|---|
| Include | Traz os dados relacionados, como o responsável. |
| ThenInclude | Traz o parentesco de cada familiar. |
| Where | Filtra os resultados. |
| Contains | Procura um trecho do nome. |
| OrderBy | Ordena os registros. |
| ToList | Executa a consulta e devolve uma lista. |
| SingleOrDefault | Busca um registro ou devolve null se ele não existir. |
| AsNoTracking | Indica que a consulta é apenas para leitura. |

Em `child => child.Id`, o nome à esquerda da seta representa o registro que está sendo analisado. À direita fica a informação usada na consulta. Essa forma de escrever é comum no Entity Framework e ainda aparece nos filtros e nas configurações.

## Como funciona a edição

GetById busca a criança e seus dados relacionados. O serviço altera esse mesmo objeto e chama Update. O contexto acompanha o que foi alterado, incluído e removido. Por isso, Update usa SaveChanges: ele não precisa repetir uma instrução de alteração para cada campo.

Essa parte é intermediária. O ponto principal para usar corretamente é manter o mesmo contexto entre a busca e a gravação.

## O tratamento de erros

O bloco try tenta gravar os dados. Se a gravação falhar, o catch limpa as alterações pendentes e repassa o erro com throw. Isso evita que o próximo comando tente salvar dados que ficaram pendentes por causa da falha anterior.

O registro de atividades usa seu próprio contexto. Assim, uma falha ao registrar o histórico não deixa alterações pendentes no contexto do cadastro.

## O que você precisa explicar na apresentação

Explique para que serve cada tabela, o que são chave primária e chave estrangeira, como o cadastro é salvo e o que acontece quando uma criança é excluída. Mostre também por que Outro tem Id 9 e por que CPF e telefone são textos.

Os arquivos da pasta Migrations são gerados pelo Entity Framework. Eles contêm os comandos de criação do banco e naturalmente são mais longos. Para entender o sistema, comece pelas classes e pelos métodos do cadastro.
