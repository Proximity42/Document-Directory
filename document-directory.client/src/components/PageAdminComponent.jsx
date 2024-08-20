import React, { Component } from 'react';
import { useEffect, useState } from 'react';
import { BrowserRouter, NavLink } from 'react-router-dom';
import { useNavigate, Navigate } from 'react-router-dom';
import { Divider, Menu, Switch } from 'antd';
import { UserOutlined } from '@ant-design/icons';

import ListUserComponent from './ListUserComponent';


function PageAdminComponent() {
    const [isShowListUser, setIsShowListUser] = useState(false);
    const [roles, setRoles] = useState([]);

    const handleChange = (item) => {

        if (item.key === '1') {
            setIsShowListUser(true);
        }
        else {
            setIsShowListUser(false);
        }
    }

    

    const menuItems = [
        {
            key: '1',
            icon: <UserOutlined />,
            label: 'Все пользователи',
        },
        {
            key: '2',
            label: 'Все группы',
        }
    ]

    return (
        <>
            <h1>Управление</h1>
            <div style={{ display: 'flex' }}>
                <div>
                    <Menu id='menu'
                        style={{
                            width: 256,
                        }}
                        defaultSelectedKeys={['1']}
                        onClick={handleChange}
                        items={menuItems}
                    />
                </div>
                <div>
                    {isShowListUser && <ListUserComponent /> }
                      
                </div>
            </div>
            
        </>
    )
}
export default PageAdminComponent;