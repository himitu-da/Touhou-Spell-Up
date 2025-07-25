# ChangeLog

## [v0.0] - 2025-06-??
- GitHubリポジトリの作成
- unity 6000.1.8fのインストール
- unityプロジェクトの作成

## [v0.1] - 2025-06-24
- 最初のビルド
- シーンとプレハブの作成
- コントローラーの設定（Input System）
- 難読化の採用（C++コンパイル）
- 自機、敵機、自機ショット、敵機弾幕をスポーンさせるように
- 自機は移動可能で、ショットを打てる
- 敵機は弾幕を放つ
- 被弾すると自機敵機はそれぞれ消滅する

## [v0.1.1] - 2025-06-25
- シングルトンパターンでSystemManagerを採用
- フレームレートを60FPS固定
- 被弾時にリスタートするように
- フォルダ構造の調節
- 敵機体力の実装（自機ショットによる減少処理も含めて）
- 敵体力の表示（円形のゲージ）
- 弾幕において、compositeパターンを採用し、複雑な弾幕パターンを作成可能に（シーケンス弾幕パターン）
- 固定n-way弾パターンを作成
- UniTaskを導入

## [v0.1.2] - 2025-06-28
- 自機にシングルトンパターンを採用
- `NWayShotPattern` を、より汎用的で強力な `MultiWayPattern` へとリファクタリング。特定の弾プレハブだけでなく、あらゆる弾幕パターンをN-Way化することが可能
- 新しい弾幕パターンとして、子パターンをN-Way状に展開する `MultiWayPattern` を追加。
- MultiWayPatternに「全方位」オプションを追加
- 弾幕を並列実行するParallelPatternを追加
- 自機狙いを作成
- 移動可能な範囲を指定できるように
- 弾幕の自機狙い機能において、弾の向きが90度ずれる問題を修正。
- 大玉を作成
- 敵機体力を少なく
- EnemyControllerからintervalの責務を削除し、代わりにLoopPatternを作成
- LoopPatternによって、非同期弾幕に対応（並列で異なるパターンの弾幕を発射）
- TextMeshPro導入

## [v0.1.3] - 2025-06-29
- ScatteringPatternを作成。ばらまき弾を打てるように
- AngleUtilityを作成。向きの取得が簡単に
- TouhouSpellUp.Danmaku NameSpaceを作成。時計回りと反時計回りを指定可能に
- 既存コードのAngleUtilityへの置き替え（MultiWayPattern）
- MultiWayPatternに対して、自機外しを設定できるように
- MultiWayPatternに各弾ごとの遅延時間と回転方向を設定できるようにする
- 敵機が回転しまくってしまう問題を解消
- 設計思想を、「何を」「どう打つか」に分別。「何」はPrefab、「どう」はBulletPatternBaseが担当
- 各パターンにおいて、使う弾を上書きできるように。BulletPatternBaseを変更
- Patternの命名規則を統一し、ファイル名を変更
    - `BasicShotPattern` → `BASIC_`
    - `LoopPattern` → `LOOP_`
    - `MultiWayPattern` → `NWAY_`
    - `ParallelPattern` → `PARA_`
    - `ScatteringPattern` → `SCTR_`
    - `SequencePattern` → `SEQ_`
- ScatteringPatternやMultiWayPatternは末端パターンとする
- 弾幕や自機、敵機のサイズを調整
- 敵弾を半透明に（一時的）
- 当たり判定の大きさを調節
- 高速移動、低速移動を実装
- 低速移動時、自機の当たり判定を表示（hitboxmarkerを追加することによって）
- 画面外に出れないようにする（移動領域の制限を実装）
- 背景色を灰色に

## [v0.1.4] - 2025-07-05
- CHANGELOGとREADMEに実装計画を分離
- menuNameの構造を調節
    - 弾幕ゲーム関係はTouhou Spell Up -> DanMakuに移動
- BulletPatternBaseをShootPatternBaseに改称
- ShootPatternsBase抽象クラスに具象クラスで共通していた処理を移動
- BulletPropertyを用いて、基本的な速度・方向などを保持するクラスを作る（BulletPatternBaseにもフィールド追加）
    - BulletPrehabはSpriteRenderer, Clider2D, Rigidbody2Dコンポーネントを保持
    - BulletPropertyはパラメータ（色、サイズ、速度）の状態を保有
    - MovePatternBaseでは「どう移動するか」
    - ShootPatternBaseでは「どう撃つか」
- BulletProperty.csとBullet.csのScriptableObjectを作成（それぞれBLTP_、BLT_）
- ShootPatternスクリプトの弾は、GameObjectを扱う形式からBulletを参照する形式に変更
- EnemyBulletを修正し、BulletPropertyからパラメータを変更できるように
- EnemyControllerを修正し、攻撃パターンの基準とするBulletアセットを渡すように
- ファイルのディレクトリ構造の変更（Bullets, Movepatternsなどを追加）
- 回転全方位パターンを作れるように（RotatingShotPattern）
- 複数のパターンでリソースを共有するためのSharedResource ScriptableObjectを作成
- 角度を共有するためのSharedAngle ScriptabelObjectを作成
- SharedResourceらを格納するためのフォルダを作成
- RotatingShotPattern上での処理を変更
- RotatingShotPattern上ではオフセット等を指定可能
- enemyのレイヤー変更（弾幕より前面に）

## [v0.1.5] - 2025-07-06
- 射撃機能の共通化。Shooter Componentを作成し、GameObjectにShooterをアタッチするだけで射撃能力を付与できるように
- 発射地点の柔軟化。ShootPatternBaseにSpawnPointTypeとpositionOffsetを追加。弾幕の起点を変更できるように
- ShootPatternBaseがShooter Componentを使うように変更（引数transformをshooterに変更）
    - ShootPatternBaseの具象クラスもShooter Componentを使うように変更
- 弾の初期化処理をShooterに移譲
- ShootPatternBaseに発射位置を変更するためのenumを作成（敵の相対位置、絶対位置、自機の相対位置）
- Shooter Componentの追加に伴うEnemyController等のリファクタリング
    - Enemy Controllerから射撃関連のコードを削除し、責任を限定化
- 移動機能の共通化。Mover Componentの作成し、GameObjectにMoverをアタッチするだけで移動能力を付与できるように
- MovePatternBaseの作成。敵機や敵弾に関する動的で複雑な「移動」ふるまいを実現する（敵機、敵機弾の双方）
- 一定方向に一定時間移動するStraightMoveを実装
- すべてのパターンの基底クラスのPatternBaseを作成
- 弾の発射可能なパターンのための基底クラス ShootablePatternクラスを作成
- MovePatternとShootPatternを、共通の`PatternBase`を継承するようにアーキテクチャを刷新。
    - `SequencePattern`, `ParallelPattern`, `LoopPattern`内で、移動パターンと射撃パターンを混在させることが可能に
- `Mover`と`Shooter`が、新しい`PatternBase`を直接扱えるように修正。
- `SequencePattern`と`ParallelPattern`が、ステップごとに弾を上書きできる機能を維持したままリファクタリング。
- Patternsフォルダを作成し、MovePattern、ShootPatternフォルダを移動
- AseetsはPatterns直下のみにし、スクリプトは各フォルダの直下に配置するように変更
- Patterns/Compositeを作成し、SequencePattern、ParallelPattern、LoopPatternはComposite直下に移動
- SequencePattern、ParallelPattern、LoopPatternのmenuNameをTouhou Spell Up/Danmaku/Compositeに変更
- PatternBase.csとShootablePattern.csをPatterns直下に移動
- 最終的なディレクトリ構造
    DanmakuGame/
    ├─ Mover.cs
    ├─ Shooter.cs
    ├─ DanmakuGameManager.cs
    └─ Patterns/
        ├─ PatternBase.cs
        ├─ ShootablePattern.cs
        ├─ Assets/
        ├─ Composite/
        ├─ Move/
        └─ Shoot/
- menuNameのDanmaku/Bullet PatternをShootに変更
- MoverにShooterを指定できるようにして、それが責任をもって発射するように
- ShootPatternbaseを実現する具象クラス（BasicShotPattern、MultiWayPattern、ScatteringPattern）に対して、向きのオフセットを指定できるように
    - 敵機完全固定弾を作れるように
- 射撃システムとの連携（射撃が一巡してから移動等）

## [v0.1.6] - 2025-07-25
- LoopPatternでループ回数の指定ができるように
- 「一定時間待つ」というようなForgetPatternを追加する（PatternBaseの具象クラスとして）
- 「中身の実装に関わらず次に進める（撃ちっぱなし）」のためのFireAndForgetPatternを追加する

- BulletPropertyにMovePatternを追加できるように
- EnemyBulletではなく、MoverがBulletの移動責務を担うように変更
- BulletプレハブにMoverコンポーネントを動的に配置するように
- CurveMovePattern（MovePatternBaseの具象クラス）を追加
- CurveMovePatternは既存の移動速度と初期角度を上書きすることもできるように
- EnemyBulletがBulletPropertyを保有するように
- へにょり弾を打てるように

- RotatingPatternにおいて、shotCountが0のとき無限に撃つように変更

- ShootPatternBaseにおいて、射撃位置を常に敵機に追随するかを選べるように（撃つのに時間がかかる弾幕だと、移動中だと射撃位置が残留してしまう
- ShootPatternBaseに自機狙いを指定する項目を増やす
- ShootPatternBaseの具象クラスに作っていた自機狙いを指定するメンバを削除し、基底クラスのものを使用するように変更

- BulletPropertyにShootPatternを追加できるようにして、弾が弾幕を打てるように
- PatternBaseに「実行前の待機時間」と「実行後の待機時間」を設定する項目を追加
- ForgetPatternは「何もしない処理」に変更
- ShootablePatternにおいて、`Execute`メソッドが`base.Execute`を呼び出すように修正

- 特定の場所を中心にして周回するSatelliteMovePatternを追加
- オーバーライドできるようにMovePatternBaseのexecuteをvirtualに変更
- MovePatternBaseのExecuteImplメソッドのシグネチャを更新
- CurveMovePatternとStraightMovePatternのオーバーライドを追加
- ShootPatternBaseに対して、spawnRadiusを追加し、発射地点からの距離を追加（GetPolerOffsetメソッドまわりで実装）
- ShootPatternBaseの具象クラスに対して、spawnRadiusを踏まえた発射地点からの処理を追加するための処理を追加

- すべての向きにおいて、方向の基準を上方向（transform.up）に統一
    - StraightMovePatternをローカル基準の移動に変更し、directionを削除

- ShootablePatternを削除し、これを継承していたクラスはPatternBaseを継承するように変更
- 弾幕を発射した場合、その発射を行ったShooterを渡すようにする（親子関係）
- 撃つときの移動に関して、初期方向と初期位置は「ShootPatternBase」、初期以外は「MovePatternBase」に統一
- SharedResourceをMultiWayPatternのDirectionOffsetでも使用できるように