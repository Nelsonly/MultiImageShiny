# MultiImageShiny
#### 多图片扫光的一个解决方案

![image-20240223173251682](https://raw.githubusercontent.com/Nelsonly/pictures/main/img/202402231732125.png)

这里感谢@[mob-sakai](https://github.com/mob-sakai/MeshEffectForTextMeshPro/commits?author=mob-sakai)开发的UIEffect，在此基础上实现了多图片按纹理扫光。

主要用到2个技术：

1. 多Image公用一个材质球
2. 设置一个世界坐标通过纹理的坐标和世界坐标距离计算光强度

##### 额外说明：

这里的世界坐标的获取是通过一个MoveListener实现，在Update里实现监听的，所以当扫光结束的时候最好Disable这个Listener。
