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
        _stateManager.AddState(new MenuState(_stateManager, _inputManager));
        _stateManager.AddState(new PongState(_stateManager, _inputManager));

        _stateManager.SwitchState("MenuState");
    }
}