using UnityEngine;

public class NPCPin : MinimapPin
{
    private Npc _npc;
    private SpriteRenderer _pin;
    void Start()
    {
        _npc = GetComponentInParent<Npc>();
        _pin = GetComponent<SpriteRenderer>();
        //아이디 넘겨서 아이디에 맞는 컬러 셋팅
        SetPinColor(_npc.id);
    }

    // Update is called once per frame
    protected override void LateUpdate()
    {
        base.LateUpdate();
    }

    private void SetPinColor(int id)
    {
        switch (id)
        {
            case 100: //대장장이
                _pin.color = Color.yellow; break;
            case 200: //요리
                _pin.color = Color.magenta; break;
            case 300: //잡화
                _pin.color = Color.gray; break;
            case 400: //여관
                _pin.color = Color.green; break;
            case 500: //검술
                _pin.color = Color.blue; break;
        }
    }
}
