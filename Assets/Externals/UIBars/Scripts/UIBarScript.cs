﻿//This is the main controlling script foe the UIBars

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIBarScript : MonoBehaviour {

	private GameObject FillerGO;
    private GameObject MaskGO;
    private GameObject PercentTxtGO;
    private GameObject RatioTxtGO;
    private Image FillerImage;
    private Text PercentTxt;
    private Text RatioTxt;
	private RectTransform FRT;
	private RectTransform MRT;

    //fill style
    public enum FillStyles{horizontal =0, vertical = 1};
	public FillStyles FillStyle = FillStyles.horizontal;

	//Mask offsets
	private Vector3 Mask0;
	private Vector3 Mask1;
	public float MaskOffset;

	//the HP that is, and the HP it will update to
	private float Value = 0.5f;
	public float NewValue = 0.5f;

	//the gradients
	public Gradient HPColor;
	public Gradient TextColor;

	//speed
	public float Speed = 10f;

	//Text bools
	public bool DisplayPercentTxt;
	public bool DisplayRatioTxt;

	[HideInInspector]//this hides the StartUpdate variable
	public bool StartAnimate = false;

    //Categories
    public enum Categories {increase = 0, decrease =1, NA = 2}
	[HideInInspector]
	public Categories UpdateCategory = Categories.decrease;
	
	//Rule Lists
	public List<CriteriaRule> CriteriaRules = new List<CriteriaRule>();
	public List<UpdateAnimationRule> UpdateAnimationRules = new List<UpdateAnimationRule>();

    private int currentHP;
    private int maxHP;
    private readonly string slash = "/";

    //Sets the variables
    void Awake () 
	{
		FillerGO = transform.GetChild(1).GetChild(0).gameObject;
		MaskGO = transform.GetChild(1).gameObject;
		PercentTxtGO = transform.GetChild(3).gameObject;
		RatioTxtGO = transform.GetChild(4).gameObject;
		FillerImage = FillerGO.GetComponent<Image>();
        PercentTxt = PercentTxtGO.GetComponent<Text>();
        RatioTxt = RatioTxtGO.GetComponent<Text>();
        FRT = (FillerGO.transform as RectTransform);
        MRT = (MaskGO.transform as RectTransform);

        //this is the location of the filler object when the HP is at 1
        Mask0 = new Vector3(FRT.localPosition.x,FRT.localPosition.y,FRT.localPosition.z);

		//the location of the filler object when the HP is at 0 depends on the FillStyle
		if (FillStyle == FillStyles.horizontal)
			Mask1 = new Vector3(FRT.localPosition.x + FRT.rect.width,FRT.localPosition.y,FRT.localPosition.z );
		else
			Mask1 = new Vector3(FRT.localPosition.x ,FRT.localPosition.y + FRT.rect.height ,FRT.localPosition.z );
	}

	void Update () 
	{
		//set the Update Category (is the HP going up, down, or not moving)
		if (Mathf.Round(Value * 100f)/100f == Mathf.Round(NewValue * 100f)/100f)
		{
			UpdateCategory = Categories.NA;
		}
		else if (Value > NewValue)
		{
			UpdateCategory = Categories.decrease;
		}
		else if  (Value < NewValue)
		{
			UpdateCategory = Categories.increase;
		}

		//Update the Mask locations (needed if you are going to move stuff arround)
		if (FillStyle == FillStyles.horizontal)
		{
			Mask1 = new Vector3(MRT.localPosition.x,MRT.localPosition.y,MRT.localPosition.z);
			Mask0 = new Vector3(MRT.localPosition.x - MRT.rect.width + MaskOffset,MRT.localPosition.y,MRT.localPosition.z );
		}
		else
		{
			Mask1 = new Vector3(MRT.localPosition.x,MRT.localPosition.y,MRT.localPosition.z);
			Mask0 = new Vector3(MRT.localPosition.x ,MRT.localPosition.y - MRT.rect.height + MaskOffset,MRT.localPosition.z );
		}

		//move the Current Value to the NewValue
		Value = Mathf.Lerp(Value,NewValue, Speed * Time.deltaTime);
		Value = Mathf.Clamp(Value,0f,1f);//make sure the Value is between 0 and 1

		//move the Filler position to display the Correct Percent
		FRT.localPosition = Vector3.Lerp (Mask0,Mask1,Value);

        //set the color for the Fill Image, and the Text Objects
        FillerImage.color = HPColor.Evaluate(Value);
        PercentTxt.color = TextColor.Evaluate(Value);
        RatioTxt.color = TextColor.Evaluate(Value);

		//Execute Each Criteria Rule
		foreach (CriteriaRule CR in CriteriaRules)
		{
			if (CR.isImage())
			{
				CR.DefaultColor = HPColor.Evaluate(Value);
			}
			else
			{
				CR.DefaultColor = TextColor.Evaluate(Value);
			}

			CR.Use(Mathf.Round(Value * 100f)/100f);
		}

		//Execute Each Update Animation Rule
		foreach (UpdateAnimationRule UAR in UpdateAnimationRules)
		{

			if (StartAnimate)
			{
				if (UAR.Category.ToString() == UpdateCategory.ToString() )
				{
					UAR.StartAnimation = true;
				}

			}

			UAR.Use();
		}

		//reset the StartAnimate variable
		StartAnimate = false;

		//activate or inactivate the text objects
		PercentTxtGO.SetActive(DisplayPercentTxt);
		RatioTxtGO.SetActive(DisplayRatioTxt);

        //update the PercentTxt 
        PercentTxt.text = (Mathf.Round(Value * 100f)).ToString() + "%";
	}

	//one way to update the UIBar
	public void UpdateValue(int newHP, int newMaxHP)
	{
		maxHP = newMaxHP;
        currentHP = newHP;
        //this will set the RatioTxt
        RatioTxt.text = newHP.ToString() + slash + newMaxHP.ToString(); 

		//NewValue
		NewValue = (float) newHP/newMaxHP;

		//trigger the start of the animation
		StartAnimate = true;
	}
	
	//an other way to update the UIBar
	public void UpdateValue(float Percent)
	{
		currentHP = (int)(maxHP * Percent);
        RatioTxt.text = currentHP.ToString() + slash + maxHP.ToString();

        //NewValue
        NewValue = Percent;

		//Trigger the start of the animation
		StartAnimate = true;
	}
	

}
