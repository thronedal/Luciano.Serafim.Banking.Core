# Luciano.Serafim.Banking.Core

Solução para core banking.

## Estrutura

A solução é desenvolvida em .Net 8, e utiliza banco de dados mongoDB.

A solução foi criada utilizando os conceitos de clean architeture:

1. Core - núcleo do sistema contendo regras de negócio e abstrações para conectividade (tanto p/ entrypoint quanto para infrastructure)
   - Abstrações, 
   - models/entidades,
   - UseCases, e
   - Classes utilitárias
2. Entrypoint - iniciadores de processamento
   - API`s (controllers)
   - Background workers (jobs, servless)
3. Infrastructure - Implementações de abstrações como por exemplo persistência;
   - Database
   - Storage
4. Crosscutting - projeto transversal que efetua a ligação entre as camandas;
   - Bootstrap
5. Tests
   - Unit tests

Dentro de cada um dos crupos acima cada microserviço pode ser especializado, ex.:
- Accounts: gerenciamento da conta-corrente, CRUD, associaçao de titulares, ativaçã/inativação de funcionalidades, etc..;
- People: gerenciamento de pessoas, CRUD, gestão de dados (LGPD);, etc..
- Events: gerenciamento de eventos, lançamentos de movimentação, saldo, extrato, etc..;

## executar através do compose

A api deve inciar na seguinte [url](http://localhost:8080/swagger)

```bash
docker compose -f "compose.debug.yml" up -d --build
```

## configurações

As configurações estão todas baseadas na execução via docker-compose, ou seja, utilizam o endereço de dentro da rede criada para o compose.

- [appsettings.json](Luciano.Serafim.Banking.Core.Api/appsettings.json)

Strings de conexão devem ser adicionadas a chave "ConnectionStrings" e seguir o formato abaixo para cada tecnologia: 

- [redis](https://stackexchange.github.io/StackExchange.Redis/Configuration.html)
- [mongoDb](https://www.mongodb.com/docs/drivers/csharp/upcoming/fundamentals/connection/connect/)

exemplo:

```json
{
  "ConnectionStrings": {
    "AccountDatabase": "mongodb://mongodb:27017",
    "DistributedLock": "redis,allowAdmin=true",
    "RedisCache": "redis,allowAdmin=true"
  }
}
```

## Possíveis problemas

Tive um erro ao utilizar volume para o Mongo no compose, o mongo não inicializava por problema de permissão para gravação

utilizando Ubunto 22.04.3

```yaml
  mongodb:
    image: mongodb/mongodb-community-server:latest
    ports:
      - "27017:27017"
    volumes:
      - './.data:/data/db'
```

### extraído do log

> {"t":{"$date":"2024-09-08T14:42:33.384+00:00"},"s":"E",  "c":"STORAGE",  "id":22312,   "ctx":"initandlisten","msg":"Error creating journal directory","attr":{"directory":"/data/db/journal","error":"boost::filesystem::create_directory: Permission denied [system:13]: \"/data/db/journal\""}}

### solução

dar permissão na pasta local (./.data)

```bash
sudo chmod 777 ./.data
sudo chmod 777 ./.keycloak
```
