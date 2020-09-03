#!/bin/bash
#Automatic partition mount test IO script.lzw 20180331
#get hard disk num all includ sda.
main_test="/root/main_test"
fdisk -l |grep -i "dev/sd*"|awk '{print $2}'|awk -F":" '{print $1}'|awk -F "/" '{print $3}'|sort|uniq >disk_num
cat disk_num |egrep -v "WARNING: GPT">agin_disk
sed '/^\s*$/d' agin_disk>all_disk
#No't include sda
fdisk -l |grep -i "dev/sd*"|grep -vw "/dev/sda"|awk '{print $2}'|awk -F":" '{print $1}'|awk -F "/" '{print $3}'|sort|uniq >n_a_titile_disks
sed '/^\s*$/d' n_a_titile_disks>not_sda
#sed -i '1d' all_disk;cat all_disk>not_sda
#format hdd
disk_num=`cat all_disk|wc -l`
if [ $disk_num -gt 1 ];then
	for i in `cat not_sda`;
	do 
		if [ -d /$i ];then
			echo "***Existing directory $i***"
			echo "***$i The hard disk is formatted.***"
			mount -a
		else
			echo "`date +%F" "%H:%M:%S`"
			echo "###Create a mount directory."$i"###"
			mkdir /$i
			echo "###Now format the hard disk ${i} ###"
			mkfs.xfs -f /dev/$i >/dev/null 2>&1
			echo "###${i} Hard disk formatting is completed.###"
			sleep 5
			#mount disk
			mount /dev/$i $mount_local/$i
			echo "###${i} Hard disk mount is completed.###"
			echo "--------------------------------------"
			sleep 5
			echo "/dev/$i   $mount_local/$i   xfs     defaults        0  0" >> /etc/fstab
			sleep 5
			echo "`date +%F" "%H:%M:%S`"
		fi
	done
else
	echo "***Only the sda disk exists..***"
	if [ -d /sda ];then echo "sda Folder name exists in /";else echo "create sda folder"; mkdir /sda;fi
	sleep 5
	
fi
lsblk;sleep 3
		
#get hard driver size an iozone tesing
rm -rf test_device*
fdisk -l |grep -i "dev/sd*"|awk -F":" '{print $2}'|awk -F " " '{print $1}' |sort |uniq > hard_size
disk_num=`cat all_disk|wc -l`
sed '/^\s*$/d' hard_size>capacity_size
capacitys=`head -1 capacity_size`
#num=`echo "sclae=0; $num1 / 2"|bc`
#capacity=`echo "sclae=0; $capacitys / 2"|bc`
capacity=300
if [ $disk_num -gt 1 ];then
	for i in `cat not_sda`;do echo -n "/$i/tfile0" "" >>test_device;done
	#sed -e 's/t//g' test_device > test_devices
	titile_device=`cat test_device`
	th_num=`cat not_sda|wc -l`
	#iozone test
	echo " ###Now perform the iozone disk test,write/rewrite；read/re-read；random-read/write####"
	echo "`date +%F" "%H:%M:%S`"
	$main_test/iozone3_471/src/current/iozone  -i 0 -i 1  -r 1024k  -s ${capacity}G  -t ${th_num} -F ${titile_device} -Rb $main_test/iozone_8k_test.xls
	echo "`date +%F" "%H:%M:%S`"
else
	echo " 	###Now perform the iozone disk test sda,write/rewrite；read/re-read；random-read/write####"
	echo "`	date +%F" "%H:%M:%S`"
	for i in `cat all_disk`;do echo -n "/$i/tfile0" "">>test_device;done
	#sed -e 's/t//g' test_device > test_devices
	titile_device=`cat test_device`
	th_num=`cat all_disk|wc -l`
	$main_test/iozone3_471/src/current/iozone -i 0 -i 1 -i 2 -r 1024k -s ${capacity}G -t ${th_num} -F ${titile_device} -Rb $main_test/iozone4k_8k_test.xls
	echo "`date +%F" "%H:%M:%S`"
	#echo "Plese check The number of hard drives is greater than the number of processors. "
	#echo "Call RD engineer"

fi




		
