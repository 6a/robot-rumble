using RR.Models.ModelLocator.Impl;
using RR.Models.ScoresModel.Impl;
using RR.Models.NetworkModel.Impl;
using RR.Models.ShuttersModel.Impl;
using RR.Models.GamestateModel.Impl;
using RR.Models.UIShakerModel.Impl;
using RR.Models.ArenaModel.Impl;
using RR.Models.LoginUIModel.Impl;
using RR.Models.MainMenuUIModel.Impl;
using RR.Models.NetPlayerModel.Impl;
using RR.Models.PlayerCameraModel.Impl;
using RR.Models.UIEventListenerModel.Impl;
using RR.Models.DBModel.Impl;
using RR.Models.CreateAccountUIModel.Impl;
using RR.Models.ModalUIModel.Impl;
using RR.Models.HUDModel.Impl;
using RR.Models.PostGameUIModel.Impl;
using RR.Models.OptionsUIModel.Impl;
using RR.Models.LeaderboardsUIModel.Impl;

public class ApplicationModelLocator : ModelLocator
{
    protected override void CreateModels()
    {
        AddModel<GamestateModel>();
        AddModel<NetworkModel>();
        AddModel<NetPlayerModel>();
        AddModel<ScoreModel>();
        AddModel<ShuttersModel>();
        AddModel<UIShakerModel>();
        AddModel<ArenaModel>();
        AddModel<LoginUIModel>();
        AddModel<MainMenuUIModel>();
        AddModel<PlayerCameraModel>();
        AddModel<UIEventListenerModel>();
        AddModel<DBModel>();
        AddModel<CreateAccountUIModel>();
        AddModel<ModalUIModel>();
        AddModel<HUDModel>();
        AddModel<PostGameUIModel>();
        AddModel<OptionsUIModel>();
        AddModel<LeaderboardsUIModel>();
    }
}
