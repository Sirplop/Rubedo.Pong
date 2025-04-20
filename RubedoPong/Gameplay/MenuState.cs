using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhysicsEngine2D;
using Rubedo;
using Rubedo.Components;
using Rubedo.Object;
using System.Diagnostics;

namespace RubedoPong.Gameplay;

/// <summary>
/// I am MenuState, and this is my summary.
/// </summary>
public class MenuState : GameState
{
    AABB button1AABB;
    AABB button2AABB;

    public MenuState(StateManager sm, InputManager ih) : base(sm, ih)
    {
        _name = "MenuState";
    }

    public override void Enter()
    {
        base.Enter();

        Texture2D button = AssetManager.LoadTexture("button");
        Sprite buttonSprite1 = new Sprite(button);
        Sprite buttonSprite2 = new Sprite(button);

        SpriteFont font = AssetManager.LoadFont("consolas");
        Text text1 = new Text(font, "Play", Color.White, true, true);
        Text text2 = new Text(font, "Quit", Color.White, true, true);
        text1.SetAlignment(Text.HorizontalAlignment.Center, Text.VerticalAlignment.Center);
        text2.SetAlignment(Text.HorizontalAlignment.Center, Text.VerticalAlignment.Center);
        text1.SetShadow(2, Color.Black);
        text2.SetShadow(2, Color.Black);

        Text titleText = new Text(font, "RUBEDO PONG", Color.White, true, true);
        titleText.textSize = 3;
        titleText.SetAlignment(Text.HorizontalAlignment.Center, Text.VerticalAlignment.Center);
        titleText.SetShadow(5, Color.Gray);

        Text subtitleText = new Text(font, "W/S to move, Space to start. Score 5 to win.", Color.White, true, true);
        subtitleText.SetAlignment(Text.HorizontalAlignment.Center, Text.VerticalAlignment.Center);
        subtitleText.textSize = 0.9f;

        Entity button1 = new Entity()
        {
            buttonSprite1,
            text1
        };
        Entity button2 = new Entity(button1.transform.Position - new Vector2(0, 72))
        {
            buttonSprite2,
            text2
        };
        Entity title = new Entity(button1.transform.Position + new Vector2(0, 150))
        {
            titleText
        };
        Entity subtitle = new Entity(title.transform.Position - new Vector2(0, 60))
        {
            subtitleText
        };

        Add(button1);
        Add(button2);
        Add(title);
        Add(subtitle);

        Vector2 buttMin = button1.transform.Position - buttonSprite1.HalfWH;
        Vector2 buttMax = button1.transform.Position + buttonSprite1.HalfWH;
        button1AABB = new AABB(buttMin, buttMax);
        buttMin = button2.transform.Position - buttonSprite2.HalfWH;
        buttMax = button2.transform.Position + buttonSprite2.HalfWH;
        button2AABB = new AABB(buttMin, buttMax);
    }

    public override void Update()
    {
        base.Update();
        if (inputManager.MousePressed(InputManager.MouseButtons.Mouse1))
        {
            Vector2 mousePos = inputManager.MouseWorldPosition();
            if (button1AABB.Contains(ref mousePos))
            {
                stateManager.SwitchState("PongState");
            }
            else if (button2AABB.Contains(ref mousePos))
            {
                Pong.Instance.Exit();
            }
        }
    }
}