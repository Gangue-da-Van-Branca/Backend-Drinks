# EloDrinks API - Configuração

## 🛠️ Configuração do Ambiente

### 1. Variáveis de Ambiente

1. Copie o arquivo `.env.example` para `.env`:
   ```bash
   cp .env.example .env
   ```

2. Edite o arquivo `.env` com suas configurações:

#### Banco de Dados
- Configure a string de conexão MySQL em `ConnectionStrings__DefaultConnection`
- Certifique-se que o MySQL está rodando e o banco `EloDrink` existe

#### JWT
- Defina uma chave secreta forte em `Jwt__Key` (mínimo 32 caracteres)

#### Email (para recuperação de senha)
- Configure as credenciais SMTP em `EMAIL_*`
- Para Gmail, use "App Passwords" em vez da senha normal

### 2. Dependências

Instale as dependências do projeto:
```bash
dotnet restore
```

### 3. Banco de Dados

Execute as migrações:
```bash
dotnet ef database update
```

### 4. Executar a API

```bash
dotnet run
```

A API estará disponível em: `http://localhost:8080`

## 📁 Estrutura de Arquivos de Configuração

- `.env` - Variáveis de ambiente (não commitado)
- `.env.example` - Exemplo de configuração
- `appsettings.json` - Configurações base
- `appsettings.Development.json` - Configurações de desenvolvimento
