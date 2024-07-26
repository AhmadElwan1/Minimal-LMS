using LiMS.Tests.Presentation;

namespace LiMS.Mocks
{
    public class MockConsole : IConsole
    {
        private Queue<string> _inputQueue = new();
        private List<string> _output = new();

        public MockConsole(IEnumerable<string> input)
        {
            foreach (var line in input)
            {
                _inputQueue.Enqueue(line);
            }
        }

        public string ReadLine()
        {
            return _inputQueue.Dequeue();
        }

        public void WriteLine(string value)
        {
            _output.Add(value);
        }

        public List<string> GetOutput()
        {
            return _output;
        }

        public void ClearOutput()
        {
            _output.Clear();
        }

        // Implement other members of IConsole as needed
        public TextReader In => throw new NotImplementedException();
        public TextWriter Out => throw new NotImplementedException();
        public TextWriter Error => throw new NotImplementedException();
    }
}