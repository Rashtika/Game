using System.Numerics;
using Npgsql;
using Game.Common;
using Game.Model;

namespace Game.Service.Common
{
    public interface IGameService
    {
        Task Insert(Champion champion);
        Task<IEnumerable<Champion>> GetAll();
        Task<Champion> GetChampion(Guid id);
        Task<IEnumerable<Champion>> GetFilteredPlayers(ChampionFiltering filtering, ChampionSorting sorting, ChampionPaging paging);
        Task UpdateChampion(Champion champion);
        Task DeleteChampion(Guid id);

    }
}
