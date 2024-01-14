using UnityEngine;

public class CursorManager : MonoBehaviour{
    private GameManager _gameManager;
    private SelectionManager _selectionManager;
    
    //public Sprite defaultCursorSprite;
    public Sprite attackCursorSprite;
    public Sprite supportCursorSprite;

    private Texture2D defaultCursorTexture;
    private Texture2D attackCursorTexture;
    private Texture2D supportCursorTexture;

    private void Start()
    {
        //defaultCursorTexture = ConvertSpriteToTexture(defaultCursorSprite);
        attackCursorTexture = ConvertSpriteToTexture(attackCursorSprite);
        supportCursorTexture = ConvertSpriteToTexture(supportCursorSprite);
        Cursor.SetCursor(defaultCursorTexture, Vector2.zero, CursorMode.Auto);
        EventManager.Instance.mouseEnteredEntity.AddListener(ChangeCursor);
        EventManager.Instance.mouseExitedEntity.AddListener(DefaultCursor);
        _gameManager = GameManager.Instance;
        _selectionManager = SelectionManager.Instance;
    }

    private void ChangeCursor(Entity hoveredEntity){
        if (_gameManager.player.nation == hoveredEntity.GetNation() && hoveredEntity is Building){
            SupportCursor();
        }else if (_gameManager.player.nation != hoveredEntity.GetNation()){
            AttackCursor();
        }
    }
    
    private void AttackCursor()
    {
        if (_selectionManager.selectedDictionary.selectedTable.Keys.Count == 0){
            return;
        }
        Cursor.SetCursor(attackCursorTexture, new Vector2(attackCursorTexture.width / 2f, attackCursorTexture.height / 2f), CursorMode.Auto); 
    }
    
    private void SupportCursor()
    {
        if (_selectionManager.selectedDictionary.selectedTable.Keys.Count == 0){
            return;
        }
        Cursor.SetCursor(supportCursorTexture, new Vector2(supportCursorTexture.width / 2f, supportCursorTexture.height / 2f), CursorMode.Auto); 
    }

    private void DefaultCursor(Entity e)
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    
    private Texture2D ConvertSpriteToTexture(Sprite sprite)
    {
        Texture2D texture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point; // Optional: Set the filter mode as per your requirements

        RenderTexture renderTexture = RenderTexture.GetTemporary(
            texture.width, texture.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear
        );
        Graphics.Blit(sprite.texture, renderTexture);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTexture;

        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTexture);

        return texture;
    }



}