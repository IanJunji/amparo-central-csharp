# Changelog

Mudanças da Central Amparo (desktop C#). As versões seguem o versionamento semântico.

## [0.2.0] - 2026-11-15

### Adicionado

- Tela de login no modo API: o operador entra com usuário e senha, e a central manda o token em todas as chamadas. O nome que vai nos atendimentos é o que a API devolve.
- Se o token vencer ou a API reiniciar, a central entra de novo sozinha com o mesmo usuário.

### Alterado

- O modo API passou a exigir login, porque a API agora recusa chamadas sem token.

## [0.1.1] - 2026-11-15

### Corrigido

- Detalhe do alerta: a linha "Recebido em" e as observações de saúde do idoso apareciam cortadas. A coluna da esquerda agora cresce conforme o texto.
- Encerrar como resolvido sem observação perguntava "Encerrar?" e só depois avisava que faltava a observação. Agora avisa antes.
- O histórico mostrava ":" sobrando quando a ação não tinha observação.
- Fila: os cabeçalhos "Tipo" e "Chance de queda" apareciam cortados.

### Alterado

- Fila: dentro de cada situação, os alertas de prioridade alta aparecem antes dos de prioridade média e baixa.

## [0.1.0] - 2026-11-08

### Adicionado

- Tela de identificação do operador, que lembra o último nome usado.
- Fila de alertas com os abertos primeiro, cores por situação e atualização a cada 10 segundos.
- Tela de atendimento do alerta: dados do idoso, contatos, histórico e ações (iniciar, ligou para o idoso, ligou para o contato, acionou o SAMU, encerrar como resolvido ou falso alarme).
- Cadastro de idoso com contatos de emergência e validação dos campos.
- Modo demonstração com arquivo JSON local e botão para simular uma queda.
- Modo API, usando as rotas do contrato de integração da API.
