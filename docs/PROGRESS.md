# RECRIAR C# - Registro de progresso

Tradução do sistema RECRIAR (C, terminal) para C# (.NET 10, Windows Forms, MySQL).
Cada passo concluído é registrado aqui, na ordem em que aconteceu.

## Decisões do projeto

| Tema | Decisão |
|---|---|
| Linguagem / versão | C# no .NET 10 (LTS) |
| Interface | Windows Forms (feita por outra pessoa da equipe) |
| Banco de dados | MySQL, tabelas e migrations feitas por outra pessoa da equipe |
| Idioma do código | Inglês (pastas, classes, métodos). Textos que o usuário vê ficam em português |
| Data de cadastro | `DateTime`, aceitando digitação com `/` e com `.` |
| Código de origem | `codigo-fonte .c` (versão completa). `cfprototipo.c` é um protótipo antigo e não será traduzido |

## Divisão de responsabilidades

| Quem | Pasta | O que faz |
|---|---|---|
| Tradução do C (este trabalho) | `Recriar.Domain`, `Recriar.Application` | Entidades, regras de validação e as ações (cadastrar, listar, editar, pesquisar, excluir, exportar ficha) |
| Banco de dados | `Recriar.Infrastructure/Persistence` | DbContext, tabelas, migrations e repositórios que implementam as interfaces de `Application/Interfaces` |
| Windows Forms | `Recriar.WinForms` | Telas e botões, chamando os serviços de `Application/Services` |

## Cuidados com o banco

- Nenhuma senha ou connection string vai para o Git.
- Usar um banco com nome próprio (`recriar_dev`) no MySQL da própria máquina, nunca em servidor do trabalho.
- Antes de rodar `database update`, conferir em qual servidor e banco a connection string aponta.

---

## Passo 1 - Estrutura da solution (concluído)

**O que foi feito**
- Criada a solution `Recriar.sln` e o `.gitignore`.
- Criados 4 projetos em `src/`, todos em `net10.0` (o WinForms em `net10.0-windows`):
  - `Recriar.Domain`: o que o sistema é (entidades e enums). Não depende de ninguém.
  - `Recriar.Application`: as ações que a tela chama, mais validações e contratos (interfaces).
  - `Recriar.Infrastructure`: parte técnica (banco de dados e exportação de arquivo).
  - `Recriar.WinForms`: telas.
- Criadas as pastas vazias de cada camada (com `.gitkeep`, porque o Git não guarda pasta vazia).
- Referências entre projetos: `Application` -> `Domain`; `Infrastructure` -> `Application`; `WinForms` -> `Application` e `Infrastructure`.

**Verificação**
- `dotnet build Recriar.sln`: 0 erros, 0 avisos.

**Observações**
- O pacote Pomelo (EF Core para MySQL) não tem versão para o EF Core 10 em outubro de 2026. A pessoa do banco escolhe entre o fork `Microting.EntityFrameworkCore.MySql 10.x` e manter o EF Core 9. Nenhum pacote de banco foi instalado.
- O `Form1` e o `Program.cs` do template do WinForms foram mantidos como vieram.

## Passo 2 - Entidades (em andamento)

Dividido em: 2.1 conceito e `FamilyMember`; 2.2 `Guardian` e `EmergencyContact`; 2.3 `Child`; 2.4 revisão.
Desenho adotado: 4 classes (`Child`, `Guardian`, `EmergencyContact`, `FamilyMember`) em vez de uma struct única como no C.

### 2.1 - `FamilyMember` (concluído)
- Criado `src/Recriar.Domain/Entities/FamilyMember.cs`, tradução da struct `Familiar`.
- Campos: `Id`, `Relationship` (parentesco), `Name`, `Age`, `ChildId` (liga ao filho).
- `Domain` compila sem erros.

### 2.2 - `Guardian` e `EmergencyContact` (concluído)
- Criado `Guardian.cs`: `Id`, `Name`, `Rg`, `Cpf`, `ChildId` (campos `nomeResponsavel`, `rgResponsavel`, `cpfResponsavel` do C).
- Criado `EmergencyContact.cs`: `Id`, `Name`, `Phone`, `ChildId` (campos `contatoEmergencia`, `telefoneEmergencia` do C).
- RG, CPF e telefone são `string` (não `int`) para não perder zeros à esquerda; a regra "só números" fica nos validadores (passo 3).

## Próximos passos

2. (continua) 2.2, 2.3 e 2.4 acima
3. Validadores e helpers (`somenteNumeros`, `somenteLetras`, `validarData`, `lerIdade`, `primeiraMaiuscula`)
4. Interfaces e DTOs
5. `CadastroService` (cadastrar, listar, editar, pesquisar, excluir)
6. Exportação da ficha em `.txt`
7. README para os dois colegas
