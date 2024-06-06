using Game.Service.Common;
using Game.Common;
using Game.Model;
using Game.Repository;
using System.Numerics;

namespace Game.Service
{
    public class ChampionService : IGameService
    {
        ChampionRepository championRepository = new ChampionRepository();
        public async Task Insert(Champion champion)
        {
            champion.Id = Guid.NewGuid();
            await championRepository.Insert(champion);
        }

        public async Task DeleteChampion(Guid id)
        {
            await championRepository.DeleteChampion(id);
        }

        public async Task<IEnumerable<Champion>> GetAll()
        {
            return await championRepository.GetAll();
        }

        public async Task<Champion> GetChampion(Guid id)
        {
            return await championRepository.GetChampion(id);
        }

        public async Task<IEnumerable<Champion>> GetFilteredPlayers(ChampionFiltering filtering, ChampionSorting sorting, ChampionPaging paging)
        {
            throw new NotImplementedException();
        }
    }
}
