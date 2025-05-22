using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Rubedo;
using RubedoPong.Gameplay;

namespace RubedoPong;

public class Pong : RubedoEngine
{
    public Pong() : base() { }  

    protected override void LoadContent()
    {
        base.LoadContent();
        _stateManager.AddState(new MenuState(_stateManager));
        _stateManager.AddState(new PongState(_stateManager));

        _stateManager.SwitchState("MenuState");
    }
}