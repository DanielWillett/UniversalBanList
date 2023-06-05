using OpenMod.API.Ioc;

namespace BlazingFlame.UniversalBanList.API;
[Service]
public interface IBanListEventListenerService
{
    void Subscribe();
    void Unsubscribe();
}
