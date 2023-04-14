using System.Diagnostics;
using System.IO;
using UnityEngine;

public class StockfishEngine
{
    private Process stockfishProcess;
    private StreamWriter stockfishStreamWriter;
    private StreamReader stockfishStreamReader;

    private int depth;
    private int skillLevel;
    private int minThinkingTime;
    private int maxErrors;
    private int matchProbability;
    private bool useBook;
    private int decay;

    public void Start(AISettings aiSettings)
    {
        stockfishProcess = new Process();
        var dir = Application.streamingAssetsPath + "/Libraries/stockfish/stockfish-windows-2022-x86-64-avx2.exe";
        stockfishProcess.StartInfo.FileName = dir;
        stockfishProcess.StartInfo.UseShellExecute = false;
        stockfishProcess.StartInfo.RedirectStandardOutput = true;
        stockfishProcess.StartInfo.RedirectStandardInput = true;
        stockfishProcess.StartInfo.CreateNoWindow = true;
        stockfishProcess.Start();
        stockfishStreamReader = stockfishProcess.StandardOutput;
        stockfishStreamWriter = stockfishProcess.StandardInput;

        depth = aiSettings.depth;
        skillLevel = aiSettings.skillLevel;
        minThinkingTime = aiSettings.minThinkingTime;
        matchProbability = aiSettings.matchProbability;
        maxErrors = aiSettings.maxError;
        useBook = aiSettings.useBook;
        decay = aiSettings.decay;

        SendCommand("uci");
        SendCommand("ucinewgame");
        SendCommand($"setoption name Skill Level value {skillLevel}");
        SendCommand($"setoption name Skill Level Probability value {matchProbability}");
        SendCommand($"setoption name Skill Level Maximum Error value {maxErrors}");
        SendCommand($"setoption name Skill Level Decay value {decay}");
        SendCommand($"setoption name Minimum Thinking Time value {minThinkingTime}");
        SendCommand($"setoption name UseBook value {useBook}");
    }

    public void Stop()
    {
        SendCommand("stop");
    }

    public void Quit()
    {
        if (stockfishProcess != null && !stockfishProcess.HasExited)
        {
            stockfishStreamWriter.WriteLine("quit");
            stockfishProcess.WaitForExit();
            stockfishStreamWriter.Close();
            stockfishStreamReader.Close();
            stockfishProcess.Close();
        }
    }

    public (string, int, int) GetBestMove(string fen)
    {
        SendCommand("position fen " + fen);
        SendCommand($"go depth {depth}");

        string output = "";
        while (!output.Contains("bestmove"))
        {
            output += stockfishProcess.StandardOutput.ReadLine();
        }

        int index = output.IndexOf("bestmove") + 9;
        string bestMove = output.Substring(index, 4);
        // Check if there is a promotion
        if (output.Length > index + 4 && output[index + 4] != ' ')
        {
            bestMove += output[index + 4];
        }

        int evalValue = 0;
        int mateIn = -1;
        var tokens = output.Split(' ');
        if (tokens.Length > 0 && tokens[0] == "info")
        {
            for (int i = 0; i < tokens.Length - 1; i++)
            {
                if (tokens[i] == "cp")
                {
                    evalValue = int.Parse(tokens[i + 1]);
                }
                else if (tokens[i] == "mate")
                {
                    mateIn = int.Parse(tokens[i + 1]);
                }
            }
        }
        return (bestMove, evalValue, mateIn);
    }

    private void SendCommand(string command)
    {
        stockfishProcess.StandardInput.WriteLine(command);
        stockfishProcess.StandardInput.Flush();
    }
    public int ReadEvaluationValue(string fen)
    {
        SendCommand($"position fen {fen}");
        SendCommand("go depth 15");
        SendCommand("eval");
        string response;
        response = stockfishStreamReader.ReadLine();
        var tokens = response.Split(' ');
        if (tokens.Length > 0 && tokens[0] == "info")
        {
            for (int i = 0; i < tokens.Length - 1; i++)
            {
                if (tokens[i] == "cp")
                {
                    int value = int.Parse(tokens[i + 1]);
                    return value;
                }
            }
        }
        return 0;
    }
}