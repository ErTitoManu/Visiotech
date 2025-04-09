using Xunit;
using Moq;
using VisiotechSystemMonitor.ViewModels;
using VisiotechSystemMonitorLib.Interfaces;
using VisiotechSystemMonitorLib.Models;
using System.Collections.ObjectModel;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
namespace VisiotechSystemMonitor.Tests.ViewModels
{
    public class MainViewModelTest
    {
        private readonly Mock<IDataCollectorService> _mockDataCollector;
        private readonly Mock<IStaticService> _mockStaticService;
        private MainViewModel _viewModel;

        public MainViewModelTest()
        {
            // Inicializamos los Mocks de las dependencias
            _mockDataCollector = new Mock<IDataCollectorService>();
            _mockStaticService = new Mock<IStaticService>();

            // Configuramos el comportamiento por defecto de los mocks si es necesario
            _mockDataCollector.Setup(dc => dc.Get()).Returns(new SampleModel { CpuUse = 50, RamUse = 60 });
            _mockStaticService.Setup(ss => ss.Load()).Returns(new ObservableCollection<SampleModel>());

            // Creamos una instancia del ViewModel con los mocks inyectados
            _viewModel = new MainViewModel(_mockDataCollector.Object, _mockStaticService.Object);
        }

        // Prueba para verificar la inicialización con Mocks es correcta
        [Fact]
        public void MainViewModel_Initialization_WithMocks()
        {
            var mockDataCollector = new Mock<IDataCollectorService>();
            var mockStaticService = new Mock<IStaticService>();

            mockStaticService.Setup(s => s.Load()).Returns(new System.Collections.ObjectModel.ObservableCollection<SampleModel>());

            var services = new ServiceCollection();
            services.AddSingleton<IDataCollectorService>(mockDataCollector.Object);
            services.AddSingleton<IStaticService>(mockStaticService.Object);
            services.AddSingleton<MainViewModel>();

            var serviceProvider = services.BuildServiceProvider();

            
            var viewModel = serviceProvider.GetRequiredService<MainViewModel>();

            // Assert
            Assert.NotNull(viewModel);
            Assert.Empty(viewModel.Samples);
            mockStaticService.Verify(s => s.Load(), Times.Once);
        }

        // Prueba para verificar la inicialización del ViewModel
        [Fact]
        public void MainViewModel_Initialization_LoadsExistingSamples()
        {
            var initialSamples = new ObservableCollection<SampleModel> { new SampleModel { CpuUse = 20 } };
            _mockStaticService.Setup(ss => ss.Load()).Returns(initialSamples);

            
            var viewModel = new MainViewModel(_mockDataCollector.Object, _mockStaticService.Object);

            // Assert
            Assert.Equal(initialSamples.Count, viewModel.Samples.Count);
            Assert.Equal(initialSamples[0].CpuUse, viewModel.Samples[0].CpuUse);
        }

        // Prueba para verificar el comando StartStopCommand
        [Fact]
        public void StartStopCommand_TogglesDataCollection()
        {
            // Aseguramos que el timer esté inicialmente detenido (por defecto en MainViewModel)
             //niciamos la recopilación
            _viewModel.StartStopCommand.Execute(null);
            Thread.Sleep(600); // Esperamos un poco para que el timer pueda dispararse

            // Assert (verificamos que la recopilación de datos ocurrió al menos una vez)
            _mockDataCollector.Verify(dc => dc.Get(), Times.AtLeastOnce);
            _mockStaticService.Verify(ss => ss.Save(It.IsAny<ObservableCollection<SampleModel>>()), Times.AtLeastOnce);

             //Detenemos la recopilación
            _viewModel.StartStopCommand.Execute(null);
            _mockDataCollector.Invocations.Clear(); // Limpiamos las invocaciones previas para la siguiente verificación
            _mockStaticService.Invocations.Clear();

            Thread.Sleep(600); // Esperamos un poco para asegurarnos de que no se recopilan más datos

            // Assert (verificamos que la recopilación de datos NO ocurrió después de detener el timer)
            _mockDataCollector.Verify(dc => dc.Get(), Times.Never);
            _mockStaticService.Verify(ss => ss.Save(It.IsAny<ObservableCollection<SampleModel>>()), Times.Never);
        }

        // Prueba para verificar la actualización del intervalo del timer
        [Fact]
        public void IntervalMilliseconds_UpdatesTimerInterval_AffectsDataCollectionFrequency()
        {
            int initialInterval = _viewModel.IntervalMilliseconds;
            int newInterval = initialInterval * 2; // Establecemos un intervalo más largo

            
            _viewModel.StartStopCommand.Execute(null); // Iniciamos el timer
            Thread.Sleep(newInterval - 100); // Esperamos un tiempo ligeramente menor que el nuevo intervalo

            // Assert (verificamos que la recopilación de datos ocurrió al menos una vez con el intervalo inicial)
            _mockDataCollector.Verify(dc => dc.Get(), Times.AtLeastOnce);
            _mockStaticService.Verify(ss => ss.Save(It.IsAny<ObservableCollection<SampleModel>>()), Times.AtLeastOnce);

             //Cambiamos el intervalo
            _viewModel.IntervalMilliseconds = newInterval;
            _mockDataCollector.Invocations.Clear(); // Limpiamos las invocaciones
            Thread.Sleep(newInterval - 100); // Esperamos un tiempo ligeramente menor que el nuevo intervalo

            // Assert (verificamos que NO se haya recopilado un nuevo dato con el nuevo intervalo más largo)
            _mockDataCollector.Verify(dc => dc.Get(), Times.Never);
            _mockStaticService.Verify(ss => ss.Save(It.IsAny<ObservableCollection<SampleModel>>()), Times.Never);

             //Esperamos un poco más que el nuevo intervalo
            Thread.Sleep(200);

            // Assert (verificamos que la recopilación de datos ocurrió al menos una vez con el nuevo intervalo)
            _mockDataCollector.Verify(dc => dc.Get(), Times.AtLeastOnce);
            _mockStaticService.Verify(ss => ss.Save(It.IsAny<ObservableCollection<SampleModel>>()), Times.AtLeastOnce);

            // Finally, stop the timer for cleanup (optional, but good practice)
            _viewModel.StartStopCommand.Execute(null);
        }

        // Prueba para verificar que OnTimerElapsed agrega un nuevo Sample y guarda los datos
        [Fact]
        public void OnTimerElapsed_AddsNewSampleAndSavesData()
        {
            _mockDataCollector.Setup(dc => dc.Get()).Returns(new SampleModel { CpuUse = 70, RamUse = 80 });

            
            // Simula el evento Elapsed del timer
            _viewModel.OnTimerElapsed(null, null);

            // Assert
            Assert.Single(_viewModel.Samples);
            Assert.Equal(70, _viewModel.Samples[0].CpuUse);
            Assert.Equal(80, _viewModel.Samples[0].RamUse);
            _mockStaticService.Verify(ss => ss.Save(It.Is<ObservableCollection<SampleModel>>(list => list.Count == 1)), Times.Once);
        }
    }
}