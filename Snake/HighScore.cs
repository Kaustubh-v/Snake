using System.IO;
using System.Text;

namespace Snake
{
    public class HighScore
    {
        private string resultFileName = "result.txt";

        public int Highscore = 0;
        
        public HighScore()
        {
            if (!File.Exists(resultFileName))
            {
                Highscore = 0;
            }
            else
            {
                string line;
                StreamReader fileStreamReader = new StreamReader(resultFileName);
                if ((line = fileStreamReader.ReadLine()) != null)
                {
                    Highscore = int.Parse(line);
                }
                fileStreamReader.Close();
            }
        }

        public void SaveResult(int score)
        {
            if (score > Highscore)
            {
                Highscore = score;
                string scoreStr = string.Format("{0}\n", score);
                FileStream scoreFile = File.OpenWrite(resultFileName);
                scoreFile.Write(Encoding.ASCII.GetBytes(scoreStr), 0, scoreStr.Length);
                scoreFile.Close();
            }
        }
    }
}
