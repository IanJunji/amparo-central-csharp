# Changelog

Mudanças da Central Amparo (desktop C#). As versões seguem o versionamento semântico.

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
