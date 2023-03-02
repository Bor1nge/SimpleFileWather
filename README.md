# SimpleFileWather
 一个监控指定文件夹的小工具。

### Usage:

```
SimpleFileWatch -path <directory1,directory2,...> -filter <filter> -target <target> -move <true|false>
```

- path：要监控的文件夹，多个文件夹以逗号分隔
- filter：匹配规则，默认为 " \*\.\*"全部匹配
- move：为true时，会将文件拷贝至保存文件夹
- target：可指定保存文件夹，默认为%TEMP%



### 没什么用处

1. 审计代码的时候，可以观察一些缓存文件、临时文件的写入情况，寻找是否有可挖掘的机会点。
2. 加上文件压缩、数据回传，再隐藏窗口后台执行。妥妥的窃密程序。（我乱说的）
3. 想不到了。