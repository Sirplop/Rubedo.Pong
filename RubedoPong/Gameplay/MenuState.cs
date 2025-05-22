using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rubedo;
using Rubedo.Components;
using Rubedo.Internal.Assets;
using Rubedo.Object;
using Rubedo.UI;
using Rubedo.UI.Graphics;
using Rubedo.UI.Input;
using Rubedo.UI.Layout;
using Rubedo.UI.Text;

namespace RubedoPong.Gameplay;

/// <summary>
/// I am MenuState, and this is my summary.
/// </summary>
public class MenuState : GameState
{
    public MenuState(StateManager sm) : base(sm)
    {
        _name = "MenuState";
    }

    public override void LoadContent()
    {
        base.LoadContent();
        //AssetManager.CreateNewFontSystem("default", "fonts/Consolas.ttf");
        AssetManager.CreateNewFontSystem("default", "fonts/DroidSans.ttf", "fonts/DroidSansJapanese.ttf", "fonts/Symbola-Emoji.ttf");
    }

    public override void Enter()
    {
        base.Enter();

        //button layout group.
        Vertical vert = new Vertical();
        vert.Anchor = Rubedo.UI.Anchor.Center;

        Label titleText = new Label(AssetManager.GetFontSystem("default"), "RUBEDO PONG", Color.White, 72);
        titleText.Anchor = Anchor.Top;
        vert.AddChild(titleText);

        Panel spacer = new Panel(0, 75);
        vert.AddChild(spacer);

        Button startButton = new Button();
        Image startImage = new Image(AssetManager.LoadTexture("button"), Color.White);
        startButton.AddChild(new SelectableTintSet(startImage));
        startButton.AddChild(startImage);
        Label startText = new Label(AssetManager.GetFontSystem("default"), "Play", Color.Black, 24);
        startText.Anchor = Anchor.Center;
        startImage.AddChild(startText);
        startButton.OnReleased += (b) => stateManager.SwitchState("PongState");
        startButton.Anchor = Anchor.Center;
        vert.AddChild(startButton);

        Button quitButton = new Button();
        Image quitImage = new Image(AssetManager.LoadTexture("button"));
        quitButton.AddChild(new SelectableTintSet(quitImage));
        quitButton.AddChild(quitImage);
        Label quitText = new Label(AssetManager.GetFontSystem("default"), "Quit", Color.Black, 24);
        quitText.Anchor = Anchor.Center;
        quitImage.AddChild(quitText);
        quitButton.OnReleased += (b) => Pong.Instance.Exit();
        quitButton.Anchor = Anchor.Center;
        vert.AddChild(quitButton);

        vert.childPadding = 10;

        GUI.Root.AddChild(vert);
    }
}