# Central Amparo (desktop C#)

Aplicação Windows Forms usada pelo operador da central de teleassistência do projeto **Amparo**. A pulseira detecta a queda do idoso, a API abre um alerta e o operador atende esse alerta por aqui.

Versão: **0.1.1** (veja o [CHANGELOG](CHANGELOG.md)).

## Ficha da CitizenTech

| | |
|---|---|
| **Nome** | Amparo |
| **Cidade e bairro** | São Paulo (SP), Santana |
| **Cidadão beneficiado** | Idosos que moram sozinhos e suas famílias |
| **Problema** | O idoso que mora sozinho cai em casa e pode ficar horas no chão até alguém perceber. |
| **Como funciona** | A pulseira (ESP32 com acelerômetro) percebe a queda, ou o idoso aperta o botão de pânico, e manda as leituras para a API. A API confirma com um modelo de Machine Learning e abre um alerta. O operador da central vê o alerta, liga para o idoso ou para a família e, se precisar, chama o SAMU. A família acompanha pelo app. |
| **O que cada parte faz** | **Pulseira** (ESP32 no Wokwi): detecta a queda e tem botão de pânico. **API** (ASP.NET Core): guarda idosos, pulseiras, alertas e atendimentos. **Modelo de ML** (Python): confirma se foi queda. **Central** (C# Windows Forms): o operador atende o alerta. **App** (React Native): a família acompanha. **Servidor** (Windows Server com IIS): hospeda a API. |
| **Link do repositório** | https://github.com/IanJunji/amparo-central-csharp |
| **Indicador de impacto** | Tempo entre a queda e o primeiro atendimento da central, em minutos. A API grava a hora em que o alerta abre e a hora da primeira ação do operador. |
| **ODS conectados** | 3 (Saúde e bem-estar), 10 (Redução das desigualdades) e 11 (Cidades e comunidades sustentáveis) |

### Como o Amparo contribui para os ODS

| ODS | Meta da ONU | Como o Amparo contribui |
|---|---|---|
| **3. Saúde e bem-estar** | 3.8: acesso a serviços de saúde essenciais de qualidade para todos | A queda é detectada na hora e a central chama a família ou o SAMU. O idoso deixa de ficar horas no chão esperando alguém perceber, e o socorro chega antes das complicações. |
| **10. Redução das desigualdades** | 10.2: inclusão social de todos, independentemente da idade | O idoso que mora sozinho passa a ter a mesma segurança de quem mora acompanhado. Ele pode continuar vivendo na própria casa, com autonomia. |
| **11. Cidades e comunidades sustentáveis** | 11.1: moradia segura e acesso aos serviços básicos | A casa do idoso fica ligada à rede de atendimento da cidade (central da prefeitura, família e SAMU). O piloto começa em Santana e pode ser ampliado para outros bairros. |

## Equipe

| Nome | RM |
|---|---|
| Cilas Pinto Macedo | RM560745 |
| Ian Junji Maluvayshi Matsushita | RM560588 |
| Leandro Kamada Pesce Dimov | RM560381 |

## O problema

O idoso que mora sozinho cai em casa e pode ficar horas no chão até alguém perceber. Quando a pulseira detecta a queda, alguém precisa ver o alerta, saber quem é o idoso, ligar para a família ou para o SAMU e registrar o que foi feito. Esse é o trabalho do operador da central.

## Quem é beneficiado

Idosos que moram sozinhos e as famílias deles, começando por um piloto em Santana, São Paulo (SP). Quem usa o programa é o operador da central de teleassistência da prefeitura.

## Telas

- **Identificação:** o operador digita o nome. O app lembra o último nome usado.
- **Fila de alertas:** lista os alertas, com os abertos primeiro (em vermelho) e os em atendimento depois (em amarelo). Dentro de cada grupo, os de prioridade alta vêm antes. Atualiza sozinha a cada 10 segundos.
- **Detalhe do alerta:** dados do idoso, contatos de emergência, histórico e os botões de ação (iniciar atendimento, liguei para o idoso, liguei para o contato, acionei o SAMU, encerrar como resolvido ou falso alarme).
- **Cadastro de idoso:** nome, nascimento, bairro, endereço, observações e contatos de emergência.

Regras principais:

- A prioridade é alta para botão de pânico ou chance de queda de 80% ou mais, média a partir de 50% e baixa abaixo disso.
- Só dá para encerrar um alerta que está em atendimento. Para marcar como resolvido é preciso escrever uma observação.
- Alerta encerrado não aceita novas ações.
- No cadastro: nome sem números, idade mínima de 60 anos, bairro obrigatório, pelo menos um contato e telefone com DDD.

## Como executar

Precisa do Windows e do [.NET SDK 10](https://dotnet.microsoft.com/download).

```powershell
cd desktop-csharp
dotnet run --project CentralAmparo
```

Ou abra `CentralAmparo.sln` no Visual Studio e aperte F5.

O arquivo `CentralAmparo/appsettings.json` define de onde vêm os dados:

- `"modo": "Demo"` (padrão): usa o arquivo `dados-demo.json`, criado ao lado do executável com 3 idosos e 3 alertas fictícios. Tem o botão **Simular queda (demo)**. Para recomeçar do zero, apague esse arquivo.
- `"modo": "Api"`: usa a API do `backend-soa` no endereço de `apiUrl`.

## Tecnologias

- C# e .NET 10 com Windows Forms
- System.Text.Json para ler e gravar JSON
- HttpClient para chamar a API do backend

## Organização

- `Modelos/`: classes do domínio (Alerta, Atendimento, Idoso, ContatoEmergencia, Operador)
- `Dados/`: `IRepositorio` com duas versões, `RepositorioApi` e `RepositorioDemo`
- `Telas/`: os quatro formulários

## Versionamento

Usamos versionamento semântico (`MAIOR.MENOR.CORREÇÃO`). A versão fica no `CentralAmparo.csproj` e as mudanças no `CHANGELOG.md`. Cada mudança vai num branch próprio e entra na `main` por pull request. Cada versão publicada ganha uma tag com o número dela (por exemplo, `v0.1.0`).
