using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Rubedo;
using Rubedo.Components;
using Rubedo.Object;

namespace RubedoPong.Gameplay;

/// <summary>
/// I am PongState, and this is my summary.
/// </summary>
public class PongState : GameState
{
    Paddle leftPlayer;
    Paddle rightPlayer;
    Ball ball;
    Text leftScoreText;
    Text rightScoreText;

    public int leftScore;
    public int rightScore;

    public int scoreToWin = 5;

    private Vector2 leftStart;
    private Vector2 rightStart;

    private bool playerSendsBall;
    private float roundCooldown;
    private bool roundStarted;
    private bool gameWon;

    private SoundEffect score;
    private SoundEffect oppScore;

    public PongState(StateManager sm, InputManager ih) : base(sm, ih)
    {
        _name = "PongState";
    }

    public override void LoadContent()
    {
        base.LoadContent();
        score = AssetManager.LoadSoundFx("score");
        oppScore = AssetManager.LoadSoundFx("opponent_score");
    }

    public override void Enter()
    {
        base.Enter();

        playerSendsBall = true;
        roundStarted = true;
        gameWon = false;

        Pong.Instance.Camera.GetExtents(out float left, out float right, out float top, out float bottom);
        rightStart = new Vector2(right - 24, 0);
        leftStart = new Vector2(left + 24, 0);

        int lineTiles = 10;
        float sep = (top - bottom) / lineTiles;
        for (int i = 0; i < lineTiles; i++)
        {
            Sprite tile = new Sprite("line");
            tile.SetColor(Color.Gray);
            Entity tileEnt = new Entity(new Vector2(0, top - (sep * i) - (tile.Height * 1.5f))) { tile };
            Add(tileEnt);
        }

        ball = new Ball();
        Entity ballEnt = new Entity()
        {
            ball
        };
        Add(ballEnt);

        rightPlayer = new Paddle(ball);
        rightPlayer.isPlayer = true;
        Entity paddleEnt = new Entity(rightStart)
        {
            rightPlayer
        };
        Add(paddleEnt);

        leftPlayer = new Paddle(ball);
        leftPlayer.isPlayer = false;
        paddleEnt = new Entity(leftStart)
        {
            leftPlayer
        };
        Add(paddleEnt);

        SpriteFont font = AssetManager.LoadFont("consolas");
        leftScoreText = new Text(font, "0", Color.White);
        leftScoreText.SetAlignment(Text.HorizontalAlignment.Center);
        rightScoreText = new Text(font, "0", Color.White);
        rightScoreText.SetAlignment(Text.HorizontalAlignment.Center);
        Entity scoreEnt = new Entity(new Vector2(left * 0.5f, top - 24)) { leftScoreText };
        Add(scoreEnt);
        scoreEnt = new Entity(new Vector2(right * 0.5f, top - 24)) { rightScoreText };
        Add(scoreEnt);
    }

    public override void Exit()
    {
        base.Exit();

        leftPlayer = null;
        rightPlayer = null;
        ball = null;
        leftScoreText = null;
        rightScoreText = null;
        leftScore = 0;
        rightScore = 0;
    }

    public void TryCollideWithPaddle(Ball ball)
    {
        Vector2 pos = ball.Transform.Position;

        if (System.Math.Sign(ball.velocity.X) != System.Math.Sign(pos.X))
            return; //already traveling the correct direction.

        if (pos.X < 0)
        { //try to collide with left paddle
            if (leftPlayer.ContainsPoint(pos))
            {
                ball.HitPaddle(leftPlayer.velocity * 50);
            }
        }
        else if (rightPlayer.ContainsPoint(pos))
        {
            ball.HitPaddle(rightPlayer.velocity * 50);
        }
    }

    public void ScoreAndReset(bool leftOrRight)
    {
        if (leftOrRight)
        {
            rightScore++;
            score.Play();
        }
        else
        {
            leftScore++;
            oppScore.Play();
        }

        ball.Transform.Position = Vector2.Zero;
        ball.velocity = Vector2.Zero;

        ball.active = false;
        ball.sprite.visible = false;

        playerSendsBall = !leftOrRight;
        roundCooldown = 2;
        roundStarted = false;

        if (leftScore >= scoreToWin)
        { //left player wins
            roundCooldown = 4;
            SpriteFont font = AssetManager.LoadFont("consolas");
            Text winText = new Text(font, "Left Player Wins!", Color.Red);
            winText.SetAlignment(Text.HorizontalAlignment.Center);
            winText.textSize = 2.2f;
            Entity scoreEnt = new Entity() { winText };
            Add(scoreEnt);
            gameWon = true;
        }
        else if (rightScore >= scoreToWin)
        { //right player wins.
            roundCooldown = 4;
            SpriteFont font = AssetManager.LoadFont("consolas");
            Text winText = new Text(font, "Right Player Wins!", Color.Green);
            winText.SetAlignment(Text.HorizontalAlignment.Center);
            winText.textSize = 2.2f;
            Entity scoreEnt = new Entity() { winText };
            Add(scoreEnt);
            gameWon = true;
        }
    }

    public override void Update()
    {
        base.Update();

        leftScoreText.SetText(leftScore.ToString());
        rightScoreText.SetText(rightScore.ToString());

        if (Pong.Input.KeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape))
        {
            Pong.Instance.Exit();
            return;
        }

        if (gameWon && roundCooldown > 0)
        {
            roundCooldown -= Pong.DeltaTime;
            if (roundCooldown <= 0)
            {
                this.stateManager.SwitchState("MenuState");
            }
            return;
        }

        if (roundCooldown > 0)
        {
            roundCooldown -= Pong.DeltaTime;
            if (roundCooldown <= 0)
            {
                roundCooldown = 0;
                roundStarted = true;
                ball.sprite.visible = true;
                ball.active = true;
                leftPlayer.Transform.Position = leftStart;
                rightPlayer.Transform.Position = rightStart;
                leftPlayer.velocity = 0;
                rightPlayer.velocity = 0;
                if (!playerSendsBall)
                    ball.Send(false);
            }
        }

        if (roundStarted && playerSendsBall && Pong.Input.KeyPressed(Microsoft.Xna.Framework.Input.Keys.Space))
        {
            ball.Send(true);
            playerSendsBall = false;
        }
    }
}