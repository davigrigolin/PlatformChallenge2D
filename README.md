# Plataforma Móvel 2D

Projeto demonstrativo em Unity 2D: a plataforma começa imóvel e sem gravidade. No primeiro contato físico com o jogador, ela se desloca horizontalmente por **5 segundos** e, ao terminar, para e cai pela ação da gravidade. A ativação acontece somente uma vez.

## Requisitos

- Unity 6 (testado em estrutura compatível com Unity 6.6).
- Pacote **Input System** instalado e habilitado (o controle usa `UnityEngine.InputSystem`, não a API antiga `UnityEngine.Input`).

## Instalação

1. Crie um projeto Unity usando o template **2D Core**.
2. Extraia o conteúdo deste ZIP na pasta raiz do projeto, a mesma que contém `Assets`, `Packages` e `ProjectSettings`.
3. Se o Windows perguntar se deseja mesclar/substituir pastas, confirme.
4. Volte ao Unity e aguarde a compilação dos scripts.
5. No menu superior, acesse **Tools > Plataforma Móvel 2D > Criar ou Redefinir Cena**.
6. Abra `Assets/Scenes/PlatformChallenge.unity` e pressione **Play**.

> O comando de criação refaz a cena de demonstração. Salve uma cópia de qualquer cena importante antes de executá-lo.

## Controles

- **A / D** ou **setas esquerda / direita:** movimentar o personagem.
- **Espaço**, **W** ou **seta para cima:** pular.

## Comportamento exigido

- Antes do contato: `Rigidbody2D` da plataforma em `Kinematic`, gravidade `0` e velocidade `0`.
- No primeiro contato com o jogador (tag `Player`): inicia o movimento para a direita.
- Velocidade horizontal padrão: `1.5` unidades por segundo.
- Duração configurada no Inspector: `5` segundos.
- Ao terminar: velocidade zerada, corpo muda para `Dynamic` e gravidade `1` é ativada.
- Uma flag impede qualquer reativação, inclusive durante a queda.

## Scripts

- `TimedMovingPlatform.cs`: contato, temporizador, movimento horizontal e queda.
- `PlayerController2D.cs`: movimentação e pulo usando o novo Input System.
- `PlatformGameHUD.cs`: instruções e estado do desafio na tela.
- `PlatformChallengeSetup.cs`: gera a cena, personagem, chão e plataforma pelo menu Tools.

## Observação

Este ZIP contém os arquivos-fonte e o gerador da cena para adicionar a um projeto Unity existente. Não é um executável `.exe`.
