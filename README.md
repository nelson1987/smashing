TDD-WebAPI-Integration
# Description
Esse projeto iniciou-se por intenção de incluir as técnicas e frameworks conhecidos ao longo da carreira do desenvolvedor dono do repositório, para a criação de um portifólio.
O que não significa que esse repositório, não possa ser utilizado como um projeto modelo para demais projetos de outros usuários.
# Criação de WebApi seguindo as seguintes premissas
* Um método GET que terá cache listando todos
* Um método GET detalhando a entidade por Id
* Um método POST que salvará a entidade na persistência relacional e publicará o evento de criação
* Um Consumer que receberá o evento de criação e persistirá a entidade na persistência não relacional
* O método POST terá validação no contrato de borda

# Frameworks e Técnicas empregadas
* .NETCore
* XUnit
* Moq
* AutoFixture
* FluentValidation
* FluentAssertions
* FluentResults
* Redis
* RabbitMq
* MongoDb
* MySql
* Serilog
* Kibana
* Observabilidade
* Grafana
* K6
* Testes unitários
* Testes de Integração
* Testes de Performance
* Benchmark

# Passo-a-passo
Foi considerado a criação da aplicação utilizando TDD, da Criação do .csproj, à visualização de logs no Kibana.
Portanto, algumas percepções de arquitetura, acontecerão mais a frente, o que pode causar estranheza, mas lembrando que a premissa é: "Só criar o que for utilizar, e quando necessário, devemos refatorar".

### Criando o projeto de api
```
dotnet new webapi PostService.API
```
### Criando o projeto de testes
```
dotnet new xunit -o PostService.Tests
```
