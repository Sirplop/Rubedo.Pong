using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Rubedo;
using Rubedo.Audio;
using Rubedo.Components;
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

    private SoundPlayer bounce;
    private SoundPlayer send;

    public Ball()
    {
        bounce = new SoundPlayer("bounce", (int)DefaultMixers.Type.Effect, 2, RubedoEngine.Audio).SetRandomInfo(1f, 0.75f, 1f);
        send = new SoundPlayer("send", (int)DefaultMixers.Type.Effect, 1, RubedoEngine.Audio).SetRandomInfo(1f, 0.9f, 1.1f);
    }

    public override void Added(Entity entity)
    {
        base.Added(entity);

        sprite = new Sprite("ball");
        Entity.Add(sprite);
    }

    public override void EntityAdded(GameState state)
    {
        base.EntityAdded(state);

        state.MainCamera.ViewRect.GetExtents(out left, out right, out top, out bottom);
        left += sprite.Width * 0.5f;
        right -= sprite.Width * 0.5f;
        bottom += sprite.Height * 0.5f;
        top -= sprite.Height * 0.5f;
    }

    public override void Update()
    {
        base.Update();

        Vector2 pos = Transform.Position;
        MathV.MulAdd(ref pos, ref velocity, Time.DeltaTime, out pos);

        if (pos.Y >= top)
        {
            velocity.Y = -velocity.Y;
            pos.Y = top;
            bounce.Play();
        }
        else if (pos.Y <= bottom)
        {
            velocity.Y = -velocity.Y;
            pos.Y = bottom;
            bounce.Play();
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
        bounce.Play();
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