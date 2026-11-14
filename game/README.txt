PLATAFORMA MOVEL 2D - STARTER PARA UNITY 6

DESCRICAO
Projeto 2D demonstrativo de uma plataforma que fica parada inicialmente. No primeiro contato fisico com o jogador, ela se desloca horizontalmente durante 5 segundos. Ao terminar o tempo, interrompe o movimento horizontal, ativa a gravidade e cai. A ativacao acontece somente uma vez.

COMO INSTALAR
1. Abra ou crie um projeto 2D no Unity 6.
2. Extraia o conteudo deste ZIP na pasta raiz do projeto Unity (a pasta que contem Assets, Packages e ProjectSettings).
3. Se o sistema perguntar se deseja mesclar os arquivos, confirme.
4. Volte ao Unity e aguarde a compilacao dos scripts.
5. No menu superior do Unity, acesse Tools > Plataforma Móvel 2D > Criar ou Redefinir Cena.
6. A cena sera criada em Assets/Scenes/PlatformChallenge.unity.
7. Abra a cena, pressione Play e teste.

COMO JOGAR
- A/D ou setas esquerda/direita: movimentar o jogador.
- Espaco, W ou seta para cima: pular.
- Encoste fisicamente na plataforma amarela para ativa-la.

COMPORTAMENTO DA PLATAFORMA
- Antes do contato: Rigidbody2D Kinematic, gravidade 0 e sem movimento.
- No primeiro contato com o objeto marcado com a tag Player: inicia o movimento horizontal para a direita.
- Durante o movimento: velocidade constante de 1,5 unidade por segundo.
- Duracao: 5 segundos, controlados pelo script TimedMovingPlatform.
- Depois dos 5 segundos: velocidade zerada, Rigidbody2D passa para Dynamic e a gravidade e ativada.
- A plataforma nao pode ser ativada novamente, inclusive durante a queda.

SCRIPTS
- TimedMovingPlatform.cs: controla a ativacao unica, os 5 segundos de movimento e a queda.
- PlayerController2D.cs: permite andar e pular para testar a plataforma.
- PlatformChallengeSetup.cs: cria a cena e configura camera, jogador, chao e plataforma automaticamente.

CONFIGURACAO
Selecione o objeto Plataforma Móvel na Hierarchy para ver os parametros do script no Inspector. A duracao ja vem configurada em 5 segundos; a velocidade e a gravidade apos o movimento tambem podem ser consultadas ali.

OBSERVACOES
- Este ZIP contem os arquivos para adicionar a um projeto Unity 6 existente; nao e um executavel .exe.
- O menu de criacao gera uma nova cena e substitui a cena que estiver aberta. Salve ou faca backup de qualquer cena importante antes de usa-lo.
- O jogador e o cenario sao criados com formas simples, sem necessidade de baixar sprites ou pacotes externos.
