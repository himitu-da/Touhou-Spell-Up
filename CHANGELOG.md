# ChangeLog

## [v0.0] - 2025-06-??
- unity 6000.1.8fのインストール
- unityプロジェクトの作成
- GitHubリポジトリの作成

## [v0.1] - 2025-06-24
- 最初のビルド
- シーンとプレハブの作成
- コントローラーの設定（Input System）
- 難読化の採用
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
- `NWayShotPattern` を、より汎用的で強力な `MultiWayPattern` へとリファクタリング。これにより、特定の弾プレハブだけでなく、あらゆる弾幕パターンをN-Way化することが可能になった。
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

Developing
- BasicShotPattern、MultiWayPattern、ScatteringPatternに対して、向きのオフセットを指定できるように
- 回転全方位パターンを作れるように
- ワインダーパターンを作成
- 時間発狂やHP発狂を作れるように
- 敵機完全固定弾を作れるように
- へにょり弾を打てるように
- 敵機以外から現れる弾幕を作れるように
- 弾を動的に速度や向きを変えられるように
- 敵機複数弾幕の実装（体力がなくなると次の弾幕に移行）
- 自機ライフの実装
- ローディングシーン、タイトルシーン（スタート画面、トップ画面）を設定
- トップ画面にGame Start、Quitを設定
- BulletPropertyを用いて、基本的な速度や色などを保持するクラスを作る（BulletPatternBase等にもフィールド追加）
- ストラテジーパターンを用いて、複雑なふるまいを実現する
