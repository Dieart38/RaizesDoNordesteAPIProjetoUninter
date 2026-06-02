# 🍽️ API Raízes do Nordeste

API Back-end para rede de lanchonetes nordestinas - Projeto Multidisciplinar UNINTER

**Aluno:** Diego Soares da Silva | **RU:** 4706920

---

## 📋 Sobre o Projeto

API para gerenciar pedidos, cardápio, estoque e fidelidade da rede Raízes do Nordeste.

**Funcionalidades:**
- ✅ Cadastro e login de usuários
- ✅ Cardápio por unidade
- ✅ Criar pedidos (APP, Totem, Balcão, Web)
- ✅ Pagamento simulado (mock)
- ✅ Pontos de fidelidade

---

## 🚀 Como Rodar a API (passo a passo)

### Pré-requisitos

Instale estes programas no seu computador:

| Programa | Para que serve | Link |
|----------|----------------|------|
| **.NET 10 SDK** | Rodar a API | [Download](https://dotnet.microsoft.com/download/dotnet/10.0) |
| **PostgreSQL 16** | Banco de dados | [Download](https://www.postgresql.org/download/) |
| **PGAdmin4** | Gerenciar o banco (já vem com PostgreSQL) | - |

### Passo 1: Instalar o PostgreSQL

1. Baixe e instale o PostgreSQL
2. **Anote a senha que você definir** (você vai precisar)
3. O PGAdmin4 será instalado automaticamente

### Passo 2: Criar o banco de dados

1. Abra o **PGAdmin4**
2. Clique em **Servers** → **PostgreSQL** (digite sua senha)
3. Clique com botão direito em **Databases** → **Create** → **Database**
4. Nome: `RaizesNordesteDB`
5. Clique em **Save**

### Passo 3: Clonar o repositório

```bash
git clone https://github.com/Dieart38/RaizesDoNordesteAPIProjetoUninter.git
cd ProjetoRaizesDoNordeste/RaizesNordeste.API/RaizesNordeste.API

### Passo 4: Configurar a senha do banco (User Secrets)
No terminal, dentro da pasta do projeto, execute:

bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=RaizesNordesteDB;Username=postgres;Password=SUA_SENHA"
⚠️ Troque SUA_SENHA pela senha que você definiu no PostgreSQL

### Passo 5: Criar as tabelas do banco
bash
dotnet restore
dotnet build
dotnet ef database update
### Passo 6: Executar a API
bash
dotnet run
### Passo 7: Abrir o Swagger (documentação)
No navegador, acesse:

text
http://localhost:5148/swagger
Pronto! A API está rodando.

🧪 Testando a API
Primeiros testes (pelo Swagger)
Cadastrar usuário

Endpoint: POST /api/Auth/registrar

Body:

json
{
  "nome": "Meu Cliente",
  "email": "cliente@email.com",
  "cpf": "12345678900",
  "senha": "MinhaSenha@123",
  "consentimentoLGPD": true
}
Fazer login (copiar o token)

Endpoint: POST /api/Auth/login

Body:

json
{
  "email": "cliente@email.com",
  "senha": "MinhaSenha@123"
}
Autorizar no Swagger

Clique no botão Authorize (cadeado)

Cole: Bearer {token} (substitua {token} pelo que você copiou)

Listar unidades

Endpoint: GET /api/Unidade

Ver cardápio

Endpoint: GET /api/Unidade/{id}/cardapio

Criar pedido

Endpoint: POST /api/Pedido

📡 Endpoints Principais
Método	          Endpoint	                         O que faz	                      Autenticação
POST	         /api/Auth/registrar	             Cadastrar usuário	                      ❌
POST	         /api/Auth/login	                 Fazer login (pegar token)	              ❌
GET	           /api/Unidade	                     Listar unidades	                        ❌
GET	           /api/Unidade/{id}/cardapio	       Ver produtos da unidade	                ❌
POST	         /api/Pedido	                     Criar pedido	                            ✅ Token
GET	           /api/Pedido	                     Ver meus pedidos	                        ✅ Token
POST	         /api/Pedido/{id}/pagamento	       Pagar pedido	                            ✅ Token
GET	           /api/Fidelidade/pontos            Ver meus pontos	                        ✅ Token
💳 Pagamento Mock
Use estes números de cartão para testar:

Cartão	               Resultado
4111111111111111	     ✅ Pagamento aprovado
5555555555554444	     ❌ Pagamento recusado
Body para pagamento:

json
{
  "metodoPagamento": "MOCK",
  "cartaoMock": {
    "numero": "4111111111111111",
    "validade": "12/28",
    "cvv": "123"
  }
}
🔧 Comandos Úteis
O que fazer	Comando
Rodar a API	dotnet run
Criar/recriar banco	dotnet ef database update
Ver senha configurada	dotnet user-secrets list
Limpar e recompilar	dotnet clean && dotnet build
⚠️ Problemas Comuns
"Não consigo conectar ao banco"
Verifique se o PostgreSQL está rodando

Verifique se a senha no User Secrets está correta

Execute: dotnet user-secrets list

"A porta 5148 já está em uso"
Feche outros terminais que estão rodando a API

Ou aguarde alguns segundos e tente novamente

"Token inválido" (401)
Faça login novamente (token expira em 8 horas)

Copie o token novo

"Cardápio vazio"
Execute: dotnet ef database update

Isso vai popular o banco com unidades e produtos

📁 Estrutura do Projeto
text
RaizesNordeste.API/
├── Controllers/      # Endpoints da API
├── Application/      # Regras de negócio
├── Domain/          # Entidades e Enums
├── Infrastructure/  # Banco de dados
├── DTOs/            # Objetos de transferência
├── Middlewares/     # Tratamento de erros
├── Program.cs       # Configuração principal
└── appsettings.Development.json
🧪 Coleção Postman
Importe o arquivo RaizesNordeste.postman_collection.json no Postman para testar todos os endpoints.

Ordem recomendada:

T01 - Registrar

T03 - Login (copiar token)

T05 - Listar unidades

T06 - Ver cardápio

T07 - Criar pedido

T09 - Pagamento aprovado

📊 Banco de Dados (PostgreSQL)
As tabelas são criadas automaticamente ao rodar dotnet ef database update.

Principais tabelas:

Usuarios - Clientes e funcionários

Unidades - Lojas da rede

Produtos - Cardápio

Estoques - Controle de estoque

Pedidos - Registro de pedidos

🔗 Links
Swagger (documentação): http://localhost:5148/swagger

Repositório: https://github.com/Dieart38/RaizesDoNordesteAPIProjetoUninter.git

👨‍💻 Autor
Diego Soares da Silva
RU: 4706920
Curso: Análise e Desenvolvimento de Sistemas
UNINTER

📄 Observação
Este projeto foi desenvolvido para fins acadêmicos como parte do Projeto Multidisciplinar da UNINTER.
