using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Rubedo.Components;
using Rubedo.Object;

namespace RubedoPong.Gameplay;

/// <summary>
/// I am Paddle, and this is my summary.
/// </summary>
public class Paddle : Component
{
    public float moveRate = 2f;
    public Sprite paddleSprite;
    public bool isPlayer = false;

    private float halfHeight;
    private float halfWidth;
    private float top;
    private float bottom;

    public float velocity = 0;

    private Ball target;

    public Paddle(Ball ball) : base(true, true) 
    {
        target = ball;
    }

    public override void Added(Entity entity)
    {
        base.Added(entity);

        paddleSprite = new Sprite("paddle");
        Entity.Add(paddleSprite);

        halfHeight = paddleSprite.Height * 0.5f;
        halfWidth = paddleSprite.Width * 0.5f;
        Pong.Instance.Camera.GetExtents(out _, out _, out top, out bottom);
    }

    public override void Update()
    {
        base.Update();

        Vector2 pos = compTransform.Position;
        float Y;
        if (isPlayer)
        {
            bool downPressed = Pong.Input.KeyDown(Keys.S);
            bool upPressed = Pong.Input.KeyDown(Keys.W);
            if (downPressed && !upPressed)
            {
                velocity -= moveRate * Pong.DeltaTime;
            }
            else if (upPressed && !downPressed)
            {
                velocity += moveRate * Pong.DeltaTime;
            }
            Rubedo.Lib.Math.Clamp(velocity, -20, 20);
        }
        else
        {
            if (target.active)
            {
                Y = moveRate * Pong.DeltaTime;
                Vector2 ballPos = target.Transform.Position;
                if (pos.Y >= ballPos.Y)
                    Y = -Y;
                velocity += Y;
            }
        }
        velocity *= 0.99f;
        Y = Rubedo.Lib.Math.Clamp(pos.Y + velocity, bottom + halfHeight, top - halfHeight);
        if (Y == bottom + halfHeight || Y == top - halfHeight)
            velocity = 0;
        Transform.Position = new Vector2(pos.X, Y);

    }

    public bool ContainsPoint(in Vector2 point)
    {
        Vector2 pos = Transform.Position;

        return pos.X - halfWidth <= point.X &&
            pos.X + halfWidth >= point.X &&
            pos.Y - halfHeight <= point.Y &&
            pos.Y + halfHeight >= point.Y;
    }
}