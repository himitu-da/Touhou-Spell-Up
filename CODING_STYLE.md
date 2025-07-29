# コーディング規約

このドキュメントは、プロジェクトにおけるC#のコーディングスタイルを定義します。

## 1. 命名規則

### 1.1. 一般的な規則

-   PascalCase: クラス名、メソッド名、プロパティ名、イベント名、enum名、インターフェース名
-   camelCase: ローカル変数、メソッドの引数
-   `_`プレフィックス付きcamelCase: プライベートフィールド (`_privateField`)
-   `I`プレフィックス付きPascalCase: インターフェース名 (`IPlayer`)

### 1.2. 具体例

| 種類 | 命名規則 | 例 |
| :--- | :--- | :--- |
| クラス | PascalCase | `PlayerController` |
| メソッド | PascalCase | `CalculateScore` |
| プロパティ | PascalCase | `PlayerHealth` |
| ローカル変数 | camelCase | `currentScore` |
| 引数 | camelCase | `void SetPosition(int newPosition)` |
| プライベートフィールド | `_` + camelCase | `private int _playerHealth;` |
| publicフィールド | PascalCase | `public int PlayerHealth;` |
| インターフェース | `I` + PascalCase | `IDamageable` |
| enum | PascalCase | `GameState` |

## 2. 書式

### 2.1. インデント

-   インデントにはスペース4つを使用します。タブは使用しません。

### 2.2. 波括弧 `{}`

-   `if`, `for`, `while` などの制御構文では、処理が1行であっても必ず波括弧を使用します。
-   波括弧は、キーワードと同じ行の末尾に配置します（`{`）。

```csharp
// 良い例
if (condition)
{
    DoSomething();
}

// 悪い例
if (condition) DoSomething();
```

## 3. コメント

-   複雑なロジックや、意図が分かりにくいコードにはコメントを追加します。
-   `//` を使用した単一行コメントを基本とします。
-   XMLドキュメントコメントは、publicなAPI（メソッド、プロパティなど）に対して記述することを推奨します。

```csharp
/// <summary>
/// プレイヤーの体力を設定します。
/// </summary>
/// <param name="health">新しい体力値。</param>
public void SetHealth(int health)
{
    // 体力は0未満にはならない
    _playerHealth = Math.Max(0, health);
}
```

## 4. その他

-   `var`キーワードは、右辺から型が明らかな場合にのみ使用を推奨します。
-   早期リターン (`early return`) を活用し、ネストが深くならないようにします。

```csharp
// 良い例
public void Process(Item item)
{
    if (item == null)
    {
        return;
    }
    // ... itemを使った処理
}

// 悪い例
public void Process(Item item)
{
    if (item != null)
    {
        // ... itemを使った処理
    }
}

## 5. 座標系と角度

### 向きの基準

-   **角度**: オブジェクトの角度は、**右方向を0度**とし、反時計回りを正とします。これはUnityの標準的な角度表現（`transform.rotation`など）に準拠します。
-   **スプライトアセット**: キャラクターや弾などのスプライトは、**正面を上向き（Up）**として作成します。
-   **角度の補正**: スクリプトから角度を扱う際は、`AngleUtility.GetAngleToTarget`のように+90度のオフセットを加えることで、右向き0度の座標系と上向き正面のスプライトの向きを一致させます。

## 6. 技術選定と実装方針

### 6.1. 入力処理 (Input System)

-   プレイヤーの入力処理には、Unity標準のInput Systemパッケージを使用します。
-   `PlayerInput`コンポーネントの`Events` > `Behavior`は`Send Messages`を基本とし、`PlayerController.cs`内に`On<Action名>`の形式でメソッドを実装します。

### 6.2. 非同期処理 (UniTask)

-   非同期処理には、原則として`Cysharp.Threading.Tasks (UniTask)`を使用します。
-   `async/await`を基本とし、パフォーマンスが重要な場面や、意図的に待機しない場合に限り`Forget()`の使用を許可します。
-   `Forget()`を使用する際は、例外が発生しないことが保証されているか、意図的に無視する理由をコメントに明記してください。

### 6.3. タグ・レイヤー管理

-   `CompareTag`や`LayerMask`で用いる文字列リテラル（マジックストリング）を避けるため、タグ名やレイヤー名を定数で管理する静的クラス（例: `GameTags`, `GameLayers`）を作成し、そこから参照することを推奨します。

```csharp
// 悪い例
if (col.CompareTag("EnemyBullet")) { ... }

// 良い例
// public static class GameTags { public const string EnemyBullet = "EnemyBullet"; }
if (col.CompareTag(GameTags.EnemyBullet)) { ... }
```

### 6.4. 弾幕パターンの設計

-   **ScriptableObjectの活用**: 弾幕の動作パターン（例：n-way弾、円形弾）は、再利用性を高めるために`ScriptableObject`を継承した`PatternBase`として実装します。
-   **テンプレートメソッドパターン**: `PatternBase`は、処理の共通フロー（事前待機、実行、事後待機）を`Execute`メソッドで定義します。個別のパターンは`ExecuteImpl`抽象メソッドをオーバーライドして具体的なロジックを実装します。
-   **キャンセル処理**: `PatternBase`の`Execute`メソッドおよびその実装は、必ず`CancellationToken`を引数に取り、`await`を挟む箇所やループの先頭で`token.IsCancellationRequested`をチェックすることで、パターンの途中中断に対応できるようにします。

### 6.5. 責務の分離とカプセル化

-   **関心の分離**: 複雑な機能は、責務ごとにクラスや`ScriptableObject`へ分割します。例えば、弾幕パターンにおいては、「どこから撃つか(`EmissionShape`)」「何を撃つか(`BulletProperty`)」「どのように撃つか(`ShootPatternBase`)」を分離して実装します。
-   **プロパティの可視性**: 基底クラスで定義し、サブクラスでのみ利用するフィールドは`protected`としてカプセル化を維持します。Inspectorでの設定が必要な場合は`[SerializeField]`を併用します。
-   **ヘルパーメソッド**: 複数のサブクラスで共通して利用される計算ロジックは、基底クラスに`protected`なヘルパーメソッドとして実装し、コードの重複を避けます。

### 6.6. 共有リソースとしてのScriptableObject

-   `ScriptableObject`は、設定値やアセット参照を保持するデータコンテナとしてだけでなく、複数のオブジェクトやパターン間で状態（例: 角度、座標）を共有するための「共有リソース」としても活用します。
-   例えば、`SharedAngle`のような`ScriptableObject`を作成し、あるパターンが更新した角度を、別のパターンが参照するといった連携を可能にします。これにより、複雑な弾幕の同期や連携を、コンポーネント間の直接参照なしに実現できます。

## 7. システム設計

### 7.1. マネージャークラスの実装

-   **シングルトンと永続化**: ゲーム全体で唯一存在し、シーンを越えて永続する必要があるマネージャークラス（例: `SystemManager`, `SoundManager`）は、シングルトンパターンと`DontDestroyOnLoad`を用いて実装します。
-   **インスタンス管理**: `Awake`メソッド内でインスタンスの重複チェックを行い、重複した場合は自身を破棄する処理を必ず記述してください。
-   **初期化処理**: アプリケーション全体に関わる初期設定（フレームレートなど）は、`SystemManager`のような中心的なマネージャークラスに集約します。
