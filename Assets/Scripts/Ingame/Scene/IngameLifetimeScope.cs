using UnityEngine;
using VContainer;
using VContainer.Unity;

public class IngameLifetimeScope : LifetimeScope
{
    [Header("Player")]
    [SerializeField] private PlayerPresenter playerPresenter;
    [SerializeField] private PlayerView playerView;
    [SerializeField] private CharacterCollision playerCollision;

    [Header("Boss")]
    [SerializeField] private BossParamator bossParamator;
    [SerializeField] private BossPresenter bossPresenter;
    [SerializeField] private BossView bossView;
    [SerializeField] private CharacterCollision bossCollision;

    [Header("Effects")]
    [SerializeField] private ScreenEffectView screenEffectView;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<Player>(Lifetime.Singleton)
            .AsSelf()
            .As<IPlayerReadModel>()
            .As<IPlayerDamageable>();

        builder.RegisterComponent(playerView)
            .As<IPlayerInput>()
            .As<IPlayerView>();
        builder.RegisterComponent(playerPresenter);

        builder.RegisterInstance(bossParamator.CreateSettings());
        builder.Register<BossModel>(Lifetime.Scoped).AsSelf().As<IBossStateContext>();
        builder.Register<BossActionLibrary>(Lifetime.Scoped);
        builder.Register<BossAI>(Lifetime.Scoped);
        builder.RegisterComponent(bossView).As<IBossView>();
        builder.RegisterComponent(bossPresenter);
        builder.RegisterComponent(screenEffectView).As<IScreenEffectView>();

        builder.RegisterInstance<IPlayerCollisionSource>(new PlayerCollisionSourceAdapter(playerCollision));
        builder.RegisterInstance<IBossCollisionSource>(new BossCollisionSourceAdapter(bossCollision));
    }
}
