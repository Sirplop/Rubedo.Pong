using Microsoft.Xna.Framework;
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

    private Vector2 leftStart;
    private Vector2 rightStart;

    private bool playerSendsBall = true;

    public PongState(StateManager sm, InputManager ih) : base(sm, ih)
    {
        _name = "PongState";
    }

    public override void Enter()
    {
        base.Enter();

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
            rightScore++;
        else
            leftScore++;

        ball.Transform.Position = Vector2.Zero;
        ball.velocity = Vector2.Zero;
        leftPlayer.Transform.Position = leftStart;
        rightPlayer.Transform.Position = rightStart;
        leftPlayer.velocity = 0;
        rightPlayer.velocity = 0;

        //send the ball immediately if right player won
        if (leftOrRight)
            ball.Send(false);
        else
            playerSendsBall = true;
    }

    public override void Update()
    {
        base.Update();

        if (playerSendsBall && Pong.Input.KeyDown(Microsoft.Xna.Framework.Input.Keys.Space))
        {
            ball.Send(true);
        }

        leftScoreText.SetText(leftScore.ToString());
        rightScoreText.SetText(rightScore.ToString());
    }
}