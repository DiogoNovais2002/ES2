using Server.Services;
using Server.Data;
using Server.Models;
using Server.DTO;
using Microsoft.EntityFrameworkCore;

namespace Server.Tests.Services
{
    public class ActivityServiceTests
    {
        private ApplicationDbContext _context;
        private ActivityService _service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // novo banco por teste
                .Options;

            _context = new ApplicationDbContext(options);
            _context.Database.EnsureCreated();

            _context.Activities.AddRange(
                new Activity { Id = 1, EventId = 100, Name = "Atividade 1", ActivityStartDate = DateTime.UtcNow, ActivityEndDate = DateTime.UtcNow },
                new Activity { Id = 2, EventId = 101, Name = "Atividade 2", ActivityStartDate = DateTime.UtcNow, ActivityEndDate = DateTime.UtcNow }
            );
            _context.SaveChanges();

            _service = new ActivityService(_context);
        }
        
        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }


        [Test]
        public async Task GetAllAsync_DeveRetornarTodasAsAtividades()
        {
            var atividades = await _service.GetAllAsync();

            Assert.AreEqual(2, atividades.Count);
        }

        [Test]
        public async Task GetByEventIdAsync_DeveRetornarApenasAtividadesDoEvento()
        {
            var atividades = await _service.GetByEventIdAsync(100);

            Assert.AreEqual(1, atividades.Count);
            Assert.AreEqual("Atividade 1", atividades[0].Name);
        }

        [Test]
        public async Task CreateAsync_DeveAdicionarAtividade()
        {
            var novaAtividade = new ActivityDto
            {
                EventId = 200,
                Name = "Nova Atividade",
                Description = "Teste",
                ActivityStartDate = DateTime.UtcNow,
                ActivityEndDate = DateTime.UtcNow
            };

            await _service.CreateAsync(novaAtividade);

            var atividades = await _service.GetAllAsync();
            Assert.AreEqual(3, atividades.Count);
            Assert.IsTrue(atividades.Any(a => a.Name == "Nova Atividade"));
        }

        [Test]
        public async Task UpdateAsync_DeveAtualizarAtividadeExistente()
        {
            var dto = new ActivityDto
            {
                EventId = 100,
                Name = "Atualizada",
                Description = "Editada",
                ActivityStartDate = DateTime.UtcNow,
                ActivityEndDate = DateTime.UtcNow
            };

            var sucesso = await _service.UpdateAsync(1, dto);

            Assert.IsTrue(sucesso);
            var atualizada = await _context.Activities.FindAsync(1);
            Assert.AreEqual("Atualizada", atualizada.Name);
        }

        [Test]
        public async Task UpdateAsync_DeveRetornarFalseSeNaoEncontrar()
        {
            var dto = new ActivityDto
            {
                EventId = 999,
                Name = "Inexistente",
                Description = "Teste",
                ActivityStartDate = DateTime.UtcNow,
                ActivityEndDate = DateTime.UtcNow
            };

            var sucesso = await _service.UpdateAsync(999, dto);

            Assert.IsFalse(sucesso);
        }

        [Test]
        public async Task DeleteAsync_DeveRemoverAtividade()
        {
            var sucesso = await _service.DeleteAsync(1);

            Assert.IsTrue(sucesso);
            Assert.IsNull(await _context.Activities.FindAsync(1));
        }

        [Test]
        public async Task DeleteAsync_DeveRetornarFalseSeNaoEncontrar()
        {
            var sucesso = await _service.DeleteAsync(999);

            Assert.IsFalse(sucesso);
        }
    }
}
