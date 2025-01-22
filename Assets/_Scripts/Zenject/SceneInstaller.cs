using com.game.player;
using Zenject;
public class SceneInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<SoundFXManager>().FromComponentInHierarchy().AsSingle();
    }
}