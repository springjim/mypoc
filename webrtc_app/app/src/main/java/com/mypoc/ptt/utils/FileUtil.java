package com.mypoc.ptt.utils;

import android.content.Context;
import android.content.res.AssetManager;
import android.os.Environment;
import android.os.StatFs;

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;

public class FileUtil {
	
	private static File updateDir = null;
	private static File updateFile = null;


	/**
	 * 检查外部存储根目录下是否存在指定文件
	 *
	 * @param fileName 要检查的文件名（包括扩展名）
	 * @return 如果文件存在，则返回true；否则返回false
	 */
	public static boolean isFileExistInExternalStorage(String fileName) {
		// 获取外部存储的根目录
		File externalStorageDirectory = Environment.getExternalStorageDirectory();

		// 构建文件的完整路径
		File file = new File(externalStorageDirectory, fileName);

		// 检查文件是否存在
		return file.exists() && file.isFile();
	}


	/**
	 * 将assets 目录下文件拷入
	 * @param context
	 * @param assetFileName
	 * @param externalFileName
	 */
	public static void copyAssetToExternalStorage(Context context, String assetFileName, String externalFileName) {

		AssetManager assetManager = context.getAssets();
		InputStream inputStream = null;
		FileOutputStream fileOutputStream = null;

		try {
			// 打开assets中的文件
			inputStream = assetManager.open(assetFileName);

			// 创建目标文件路径
			File externalFile = new File(Environment.getExternalStorageDirectory(), externalFileName);

			// 创建输出流
			fileOutputStream = new FileOutputStream(externalFile);

			// 拷贝文件内容
			byte[] buffer = new byte[1024*5];
			int read;
			while ((read = inputStream.read(buffer)) != -1) {
				fileOutputStream.write(buffer, 0, read);
			}

			// 成功提示
			System.out.println("File copied to external storage: " + externalFile.getAbsolutePath());

		} catch (IOException e) {
			e.printStackTrace();
		} finally {
			// 关闭流
			try {
				if (inputStream != null) {
					inputStream.close();
				}
				if (fileOutputStream != null) {
					fileOutputStream.close();
				}
			} catch (IOException e) {
				e.printStackTrace();
			}
		}
	}

	/***
	 * 创建文件
	 */
	public static File createFile(Context context, String name) {
		if (Environment.MEDIA_MOUNTED.equals(Environment
				.getExternalStorageState())) {
			if(updateFile != null){
				return updateFile;
			}
			
			updateDir = new File(Environment.getExternalStorageDirectory().getPath());
			//updateFile = new File(updateDir + "/" + name + ".apk");
			updateFile = new File(updateDir +  File.separator+ name + ".apk");

			if (!updateDir.exists()) {
				updateDir.mkdirs();
			}
			if (!updateFile.exists()) {
				try {
					updateFile.createNewFile();
				} catch (IOException e) {
					e.printStackTrace();
				}
			}

		}
		
		return updateFile;
	}
	
	
	/**
	 * 文件重命名
	 * @param file 要重命名的文件
	 * @param newName 命名的字!
	 * @return
	 * 作者:fighter <br />
	 * 创建时间:2013-3-4<br />
	 * 修改时间:<br />
	 */
	public static boolean reNameFile(File file, String newName){
		return file.renameTo(new File(file.getParentFile(), newName));
	}
	
	/**
	 * SD卡可用容量
	 * @return 字节数
	 *  -1  SD card 读取空间错误!
	 * 作者:fighter <br />
	 * 创建时间:2013-3-4<br />
	 * 修改时间:<br />
	 */
	public static long SDCardAvailable(){
		try {
			StatFs statFs = new StatFs(getExternalDirectory());
			return (long)statFs.getBlockSize() * (long)statFs.getAvailableBlocks();
		} catch (Exception e) {
			e.printStackTrace();
			return -1;
		}
	}
	
	/**
	 * SD卡容量是否还有可用容量 ( 基数为 40MB )
	 * @return
	 * 作者:fighter <br />
	 * 创建时间:2013-4-16<br />
	 * 修改时间:<br />
	 */
	public static boolean isSDCardAvailable(){
		long volume = SDCardAvailable();
		long mb = 1024 * 1024 * 40;
		if(volume > mb){
			return true;
		}else{
			return false;
		}
	}
	
	public static String getExternalDirectory(){
		return Environment.getExternalStorageDirectory().getAbsolutePath();
	} 
	
	/**
	 * SD卡是否可用
	 * @return
	 * 作者:fighter <br />
	 * 创建时间:2013-5-6<br />
	 * 修改时间:<br />
	 */
	public static boolean isMounted(){
		return Environment.MEDIA_MOUNTED.equals(Environment.getExternalStorageState());
	}
	/**
	 * 判断两个字符串大小
	 * */
	public static boolean isSort(String str1,String str2){
		if(str1.hashCode()<=str2.hashCode()){
			return true;
		}else{
		return false;
		}
	}
}
