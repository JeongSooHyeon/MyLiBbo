using UnityEngine;
//여기서는 업그레이 비용 및 업그레이 요소 그리고 상단의 비용을 관장한다.
public class GroupContents : MonoBehaviour 
{
	[SerializeField] UILabel[] showUpgradeLabels;
	[SerializeField] UILabel[] upgradeCostLabels;
	//[SerializeField] UISprite ballSprite;
    //[SerializeField] CurrentShipShow planeChange;

	//밑에 함수는 애니메이션에서 호출한다.
	public void SetAnim() 
	{
		LobbyController.instance.SetLabel(UIState.Coin , string.Format("{0:N0}", DataManager.instance.GetCoin()));

		if(DataManager.instance.FirePower >= 100)
		{
			showUpgradeLabels[1].text = "MAX";
			upgradeCostLabels[1].text = "MAX";
		}
		else
		{
			showUpgradeLabels[1].text = string.Format("{0:N0}", DataManager.instance.FirePower);
			upgradeCostLabels[1].text = string.Format("{0:N0}", DataManager.instance.ReturenFireBallCoin);
		}
		if(DataManager.instance.StartBall >= 100)
		{
			showUpgradeLabels[0].text = "MAX";
			upgradeCostLabels[0].text = "MAX";
		}
		else
		{
			showUpgradeLabels[0].text = string.Format("{0:N0}", DataManager.instance.StartBall);		
			upgradeCostLabels[0].text = string.Format("{0:N0}", DataManager.instance.ReturenStartBallCoin);
		}
	}

	public void UpStartBall()
	{
		if(DataManager.instance.StartBall >= 100) return;
		if(DataManager.instance.GetCoin() >=  DataManager.instance.ReturenStartBallCoin)
		{
			DataManager.instance.SetCoin(-DataManager.instance.ReturenStartBallCoin);
			DataManager.instance.StartBall += 1;
			if(DataManager.instance.StartBall % 10 == 1)
			{
				DataManager.instance.BallSprite += 1;
			}
			if(DataManager.instance.StartBall >= 100)
			{
				showUpgradeLabels[0].text = "MAX";
				upgradeCostLabels[0].text = "MAX";
			}
			else
			{
				showUpgradeLabels[0].text = string.Format("{0:N0}", DataManager.instance.StartBall);
				upgradeCostLabels[0].text = string.Format("{0:N0}", DataManager.instance.ReturenStartBallCoin);
			}
			
			LobbyController.instance.SetLabel(UIState.Coin , string.Format("{0:N0}", DataManager.instance.GetCoin()));
		}
	}

	public void UpFirePower()
	{
		if(DataManager.instance.FirePower >= 100) return;
		if(DataManager.instance.GetCoin() >=  DataManager.instance.ReturenFireBallCoin)
		{
			DataManager.instance.SetCoin(-DataManager.instance.ReturenFireBallCoin);
			DataManager.instance.FirePower += 1;
			if(DataManager.instance.FirePower % 10 == 1)
			{
			
				DataManager.instance.PlaneSprite += 1;
			}
			if(DataManager.instance.FirePower >= 100)
			{
				showUpgradeLabels[1].text = "MAX";
				upgradeCostLabels[1].text = "MAX";
			}
			else
			{
				showUpgradeLabels[1].text = string.Format("{0:N0}", DataManager.instance.FirePower);
				upgradeCostLabels[1].text = string.Format("{0:N0}", DataManager.instance.ReturenFireBallCoin);
			}
		
			LobbyController.instance.SetLabel(UIState.Coin , string.Format("{0:N0}", DataManager.instance.GetCoin()));
		}
	}
}
