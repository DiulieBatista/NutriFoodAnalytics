using NutriFoodAnalytics.Models;
using NutriFoodAnalytics.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NutriFoodAnalytics.Repositories
{
    public class AlimentoRepository
    {
        private readonly AlimentoService _service;

        public AlimentoRepository()
        {
            _service = new AlimentoService();
        }

        // ───────────────────────────────────────
        // BUSCAR TODOS
        // ───────────────────────────────────────

        public async Task<List<Alimento>>
            BuscarTodosAsync()
        {
            return await _service
                .BuscarTodosAsync();
        }

        // ───────────────────────────────────────
        // BUSCAR POR NOME
        // ───────────────────────────────────────

        public async Task<List<Alimento>>
            BuscarPorNomeAsync(string nome)
        {
            var alimentos =
                await BuscarTodosAsync();

            return alimentos
                .Where(a =>
                    a.Nome != null &&
                    a.Nome.Contains(
                        nome,
                        StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }


}