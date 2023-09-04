using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPU
{
	public enum CPUType
    {
		/// <summary>自由気ままな完全ランダム（priorityの影響を受けない）</summary>
		Carefree,

		/// <summary>大きいブロックを優先</summary>
		Larger,

		/// <summary>高密度のブロックを設置</summary>
		Densitic,
	}

	/// <summary>アルゴリズムによって設置位置を決定</summary>
	/// <param name="playerIndex">決定するプレイヤー番号</param>
	/// <param name="board">ボード情報</param>
	/// <param name="hand">手札情報</param>
	/// <param name="type">CPUの種類</param>
	/// <param name="priority">優先度（CPUTypeをいかに重要視するか）</param>
	public static (int handIndex, Board setData) AI(int playerIndex, Board board, GameManager.CPUData data)
	{
		if (playerIndex == 1) OutputDebugText(board, "board[,].txt");

		// 設置可能マスの設置時の得点リスト
		var handScoreList = new List<(int handIndex, Blocks blocks, int score)>();

		for(int i = 0; i < data.hand.hand.Length; ++i)
        {
			if (data.hand.hand[i] == null) continue;

			// 設置可能マスの設置時の得点リスト
			var scoreList = new List<(Blocks blocks, int score)>();

			// 設置可能なマスを走査して評価結果を格納
			for (int y = 0; y < board.height - data.hand.hand[i].height; ++y) {
				for (int x = 0; x < board.width - data.hand.hand[i].width; ++x)
				{
					// 位置を設定した手札のブロックを複製
					Blocks blocks = new Blocks(data.hand.hand[i].shape, new Vector2Int(x, y), data.hand.hand[i].density);

					// 設置不可であれば、次のマスを調べる
					if (!blocks.IsSetable(board, playerIndex)) continue;

					scoreList.Add((blocks, Evaluate(blocks, data)));
				}
			}

			// どこにも設置できなかった場合、すべてfalseの盤面情報を作り返却
			if (scoreList.Count == 0) return (-1, new Board(0, 0));

			// 同スコアのブロックスに対してランダム化するために事前にリストをシャッフル
			scoreList.Shuffle();

			// その手札の中で一番いいブロックスを頼む
			var handResult = WhereBetter(scoreList, 0);

			// 得点リストの高得点情報をリストに格納
			handScoreList.Add((i, handResult.blocks, handResult.score));
		}

		// 一番いい手札を頼む
		var result = WhereBetter(handScoreList, 0);

		Board setData = new Board(GameManager.boardSize.x, GameManager.boardSize.y);

		// ControlBlockのDecisionStateと同じやり方で設置情報を設定
		for (int y = 0; y < result.blocks.height; ++y)
			for (int x = 0; x < result.blocks.width; ++x)
				setData.SetBoardData(result.blocks.shape[x, y].ToInt(), result.blocks.position.x + x, result.blocks.position.y + y);

		// その結果をタプル情報にして返す
		return (result.handIndex, setData);
	}

	///<summery>評価処理</summery>
	///<params="list">評価する座標</params>
	static int Evaluate(Blocks blocks, GameManager.CPUData data)
	{
        switch (data.type)
        {
            case CPUType.Larger:   return blocks.GetBlockNum();
            case CPUType.Densitic: return (int)(blocks.density * 100f);
			default:			   return Random.Range(0, 100);
        }
    }

	///<summery>リストの中から得点の降順でn番目の座標を返す</summery>
	///<params="list">設置可能マスのリスト</params>
	///<params="n">降順で何番目の座標を返すか (0 to)</params>
	static (Blocks blocks, int score) WhereBetter(List<(Blocks blocks, int score)> list, int n = 0)
	{
		return list.OrderByDescending(item => item.score).Skip(n).First();
	}

	///<summery>リストの中から得点の降順でn番目の座標を返す</summery>
	///<params="list">設置可能マスのリスト</params>
	///<params="n">降順で何番目の座標を返すか (0 to)</params>
	static (int handIndex, Blocks blocks, int score) WhereBetter(List<(int handIndex, Blocks blocks, int score)> list, int n = 0)
	{
		return list.OrderByDescending(item => item.score).Skip(n).First();
	}

	static void OutputDebugText(Board board, string filePath = "debugText.txt")
	{
		string debugText = "";

		for (int y = 0; y < GameManager.boardSize.y; ++y)
		{
			for (int x = 0; x < GameManager.boardSize.x; ++x)
			{
				int n = board.GetBoardData(x, y);

				switch (n)
				{
					case -1: debugText += "　"; break;
					case 0:  debugText += "・"; break;
					default: debugText += n.ToString().ToFullWidth(); break;
				}
			}

			debugText += "\n";
		}
		

		TextOperate.WriteFile(filePath, debugText);
	}
}
