# SelectDataTable
一个 用 C# 实现的 用 Sql select DataTable 资料 的 程序





这是一个 用 C# 实现的 用 Sql select DataTable 资料 的 程序 。   最初的 目的 是 为 面向 数据集 的 开发 提供一个方便的工具 。  
后来 衍生出了 语法分析 编译原理 的 内容 。      



面向 数据集 的 开发 ，   简单讲 可以 就是  Sql + DataTable 。    这是一种 简单直接 的 开发方式 。

Sql 是 开发人员 信手拈来 的 技能， 也是一个 成熟的 通用的 接口 。 用 Sql 操作 数据集 会 很方便 。

这个程序 支持 简单的 select 语句 ， 支持  + - * / and or not = > < >= <= !=   运算 。

支持 * 号 （表示要 select 全部 栏位） 。

支持 中括号 []  ，  比如  栏位名 和  关键字 重名， 可以用 中括号 [] 括起来 。

Sql 中 from 关键字 后面 的 表名 是 “dt”  ，  这是 固定 的 。

select 的 结果 是 一个 新的 DataTable  。

支持 用 字符串 表示 日期时间 格式 。 比如   where create_date > '2016-07-08'   。   
会判断 与之比较的 栏位 如果是 DateTime 类型 ， 则 把 字符串 转为 DateTime 类型 再进行 比较 。

计划支持 sum() count() 等 函数 ， 以及  group by   order by  ，    目前 还 没有支持 。  这些特性 应该会比较有 实用价值 。

不支持 表连接 。  基本上， 未来也不会支持。 ^^   因为 表连接 会 让事情变得复杂 。 在 应用程序层 没有索引 的 情况 下 做 表连接 会变得 效率低下 。
表连接 还是 放在 关系数据库 里 进行 比较好 。

不支持 子查询 。 子查询 也是 复杂的 。    实际中 可以 对 select 返回 的 DataTable 再 select 就行 。        

这个程序 的 核心部分 是  Sql 解析部分 。 这部分 以及可以作为一个 简单的 语法分析器 了。  ^^

Sql 解析部分 的 设计 如下 ：

有 3 个 解析类 ：  Parser, ExpressParser, SqlParser 。    

Parser 是 基本 的 解析类， 负责 分词（Word） 和 获取 包裹符（Wrapper） 内 的 内容（Content） 。

这里 的 词（Word） 不是 单词， 是指 有意义的一个文本段落 。 分词规则 根据 语法规则 而定 。

比如 在 Sql 里， 是 根据  包裹符（Wrapper） 和 白空（White Space）  来  分词  。  在 C\C++ 里， 是 根据 大括号{} 和 分号;   来 分词 。

在  SelectDataTable 里 ， 首先， 根据 包裹符 分词， 包裹符 有 3 种 ：  单引号' 中括号[] 小括号() 。

根据 包裹符 分词 以后， 可以 得到  未被包裹（NonWrapped）  的  Word ，  那么，就在 未被包裹 的 Word 里 寻找 关键字， 
比如 select from where 等 。  

然后 根据 关键字 再 分词，  比如  select 和 from 之间 的 就是  columnList Word ， where  关键字 之后的 就是 where 条件 Word 。

接下来 就 进一步 解析  columnList Word 和 where 条件 Word 。

columnList Word 的 解析比较简单， 直接 Split(',') ， 再 去掉 栏位名 外面的 中括号 。

where 条件 的 解析 由  ExpressParser  负责 。 

Express（表达式） 的 解析 同样 是 先分词（Word） ， 同样 也 先 根据 包裹符 分词 ， 然后 在 未被包裹 的 Word 中 需要 运算符 。

这是 第一遍 扫描 。

第二遍 扫描 则是 根据 运算符 来 分词（Word） ，运算符 两边 的 Word 又作为 Express（表达式） 再交给  ExpressParser 递归解析 。

SqlParser 负责 总体 的 解析 工作 。 包括 调用 Parser 和 ExpressParser ， 以及 获取 ColumnList 和 寻找 KeyWord 。   
ColumnList 是 select 关键字 后面 要 select 的 栏位列表 。

总体的 解析结果 是  返回  ColumnList 和  Express  对象嵌套 ，   没有 生成 目标代码 。 如果 生成 目标代码 ，  那就是 编译原理 了 。  呵呵


还好 C# 支持
































