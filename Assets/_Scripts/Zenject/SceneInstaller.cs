using com.game.player;
using Zenject;
public class SceneInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<PlayerStats>().FromComponentInHierarchy().AsSingle();
        Container.Bind<SoundFXManager>().FromComponentInHierarchy().AsSingle();
    }
}