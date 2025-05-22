using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Rubedo;
using Rubedo.Components;
using Rubedo.Internal.Assets;
using Rubedo.Lib;
using Rubedo.Object;

namespace RubedoPong.Gameplay;

/// <summary>
/// I am Ball, and this is my summary.
/// </summary>
public class Ball : Component
{
    public Sprite sprite;
    float left;
    float right;
    float top;
    float bottom;

    public Vector2 velocity;

    private SoundEffect bounce;
    private SoundEffect send;

    public Ball() : base(true, true) 
    {
        bounce = AssetManager.LoadSoundEffect("bounce");
        send = AssetManager.LoadSoundEffect("send");
    }

    public override void Added(Entity entity)
    {
        base.Added(entity);

        sprite = new Sprite("ball");
        Entity.Add(sprite);

        Pong.Instance.Camera.GetExtents(out left, out right, out top, out bottom);
        left += sprite.Width * 0.5f;
        right -= sprite.Width * 0.5f;
        bottom += sprite.Height * 0.5f;
        top -= sprite.Height * 0.5f;
    }

    public override void Update()
    {
        base.Update();

        Vector2 pos = Transform.Position;
        MathV.MulAdd(ref pos, ref velocity, Pong.DeltaTime, out pos);

        if (pos.Y >= top)
        {
            velocity.Y = -velocity.Y;
            pos.Y = top;
            bounce.Play(1, Random.Range(0.75f, 1f), 0);
        }
        else if (pos.Y <= bottom)
        {
            velocity.Y = -velocity.Y;
            pos.Y = bottom;
            bounce.Play(1, Random.Range(0.75f, 1f), 0);
        }

        if (pos.X >= right)
        {
            //score point for left player and reset
            ((PongState)Entity.State).ScoreAndReset(false);
        }
        else if (pos.X <= left)
        {
            //score point for right player and reset
            ((PongState)Entity.State).ScoreAndReset(true);
        }
        else
        {
            Transform.Position = pos;
            ((PongState)Entity.State).TryCollideWithPaddle(this);
        }
    }

    public void HitPaddle(float addYVelocity)
    {
        int sign = -System.MathF.Sign(velocity.X);
        velocity.X = sign * System.MathF.Min(System.MathF.Abs(velocity.X * 1.3f), 250);
        velocity.Y += addYVelocity;
        bounce.Play(1, Random.Range(0.75f, 1f), 0);
    }

    /// <summary>
    /// <paramref name="leftOrRight"/> is who we're sending the ball at.
    /// </summary>
    public void Send(bool leftOrRight)
    {
        velocity.X = leftOrRight ? -100 : 100;
        velocity.Y = Random.Range(-100f, 100f);
        send.Play();
    }
}