using UnityEngine;

public class NPCPin : MinimapPin
{
    private Npc _npc;
    private SpriteRenderer _pin;
    void Start()
    {
        _npc = GetComponentInParent<Npc>();
        _pin = GetComponent<SpriteRenderer>();
        //���̵� �Ѱܼ� ���̵� �´� �÷� ����
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
            case 100: //��������
                _pin.color = Color.yellow; break;
            case 200: //�丮
                _pin.color = Color.magenta; break;
            case 300: //��ȭ
                _pin.color = Color.gray; break;
            case 400: //����
                _pin.color = Color.green; break;
            case 500: //�˼�
                _pin.color = Color.blue; break;
        }
    }
}
